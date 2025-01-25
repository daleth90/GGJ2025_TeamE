using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.SceneManagement;

namespace Physalia.AssetManager
{
    public class CheckUpdateResult
    {
        public bool success;
        public long size;
        public ContentCatalogData remoteCatalog;
        public IResourceLocator remoteCatalogLocator;
        public string remoteCatalogHash;
        public List<string> needToUpdateKeys;

        public static CheckUpdateResult EmptySuccess => new()
        {
            success = true
        };

        public static CheckUpdateResult Failure => new();
    }

    public class CdnSetting
    {
        public string urlFormat;
        public string catalogPath;
        public string authorization;
    }

    public class AddressableAssetManager : IAssetManager
    {
        private static readonly Logger.Label Label = Logger.Label.CreateFromCurrentClass();
        private const string UrlReplacement = "http://RemoteURL";  // Important: Must start with "http://" (Yes, include "://"), to hack and trigger the remote cache loading.
        private const string BuiltInBundleFolderName = "bb";
        private const string BuiltInBundleListFileName = "bb_bundles.json";
        private const string BuiltInCatalogHashFileName = "bb_catalog.hash";
        private const string CatalogCacheFolderName = "cc";

        private readonly AssetRepository _assetRepository = new();
        private readonly Dictionary<string, SceneInstance> _sceneRepository = new(4);

        private readonly Dictionary<string, UniTask> _loadingTasks = new();
        private bool _haveSceneLoading;

        private IResourceLocator _mainCatalog;
        private string _mainCatalogHash;
        private string _buildInCatalogHash;
        private readonly HashSet<string> _builtInBundleNames = new();
        private bool _hasRemoteContent;
        private bool _isCustomRemoteUrl;
        private CdnSetting _cdnSetting;
        private string _remoteHashUrl;

        public static string BuiltInBundleFolderPath => $"{Application.streamingAssetsPath}/{BuiltInBundleFolderName}";
        public static string BuiltInBundleListPath => $"{BuiltInBundleFolderPath}/{BuiltInBundleListFileName}";
        public static string BuiltInCatalogHashPath => $"{BuiltInBundleFolderPath}/{BuiltInCatalogHashFileName}";
        public static string CatalogCacheFolderPath => $"{Application.persistentDataPath}/{CatalogCacheFolderName}";

        public bool HasRemoteContent => _hasRemoteContent;
        public bool IsCustomRemoteUrl => _isCustomRemoteUrl;
        public string MainCatalogHash => _mainCatalogHash ?? _buildInCatalogHash ?? "";

        public IEnumerable<string> ManagedKeys => _assetRepository.Keys;

        public async UniTask InitializeAsync()
        {
            // Note: Since we don't use the catalog cache by addressables, this should only load from the player.
            _mainCatalog = await Addressables.InitializeAsync(true);
            await UniTask.DelayFrame(1);  // BugFix: Classic async bug

            ValidateIfCustomRemoteUrl(_mainCatalog);
            if (_hasRemoteContent)
            {
                Addressables.InternalIdTransformFunc = TransformInternalId;
                Addressables.WebRequestOverride = OverrideWebRequest;

                await LoadBuildInCatalogHashAsync();
                await LoadBuiltInBundleListAsync();

                bool hasCatalogCache = await CheckIfCatalogCacheAvailable(_buildInCatalogHash);
                if (hasCatalogCache)
                {
                    string catalogName = Path.GetFileName(_remoteHashUrl).Replace(".hash", ".json");
                    (IResourceLocator cachedCatalog, string cachedCatalogHash) = await LoadCatalogCacheAsync(catalogName);
                    if (cachedCatalog != null)
                    {
                        Addressables.RemoveResourceLocator(_mainCatalog);
                        Addressables.AddResourceLocator(cachedCatalog);
                        _mainCatalog = cachedCatalog;
                        _mainCatalogHash = cachedCatalogHash;
                        Logger.Debug(Label, $"Load catalog from cache. Hash: {_mainCatalogHash}");
                    }
                }
            }
        }

        private async UniTask LoadBuildInCatalogHashAsync()
        {
            _buildInCatalogHash = null;

            (bool success, string text) = await LoadFileAsync(BuiltInCatalogHashPath);
            if (!success)
            {
                Logger.Error(Label, $"Load {BuiltInCatalogHashFileName} failed!");
                return;
            }

            Logger.Debug(Label, $"{BuiltInCatalogHashFileName}: {text}");
            _buildInCatalogHash = text;
        }

        private async UniTask LoadBuiltInBundleListAsync()
        {
            _builtInBundleNames.Clear();

            (bool success, string text) = await LoadFileAsync(BuiltInBundleListPath);
            if (!success)
            {
                Logger.Error(Label, $"Load {BuiltInBundleListFileName} failed!");
                return;
            }

            List<string> names;
            try
            {
                names = JsonConvert.DeserializeObject<List<string>>(text);
            }
            catch (System.Exception e)
            {
                Logger.Fatal(e);
                return;
            }

            _builtInBundleNames.AddRange(names);
        }

        private static async UniTask<bool> CheckIfCatalogCacheAvailable(string bbCatalogHash)
        {
            if (!Directory.Exists(CatalogCacheFolderPath))
            {
                return false;
            }

            string bbHashFilePath = $"{CatalogCacheFolderPath}/{BuiltInCatalogHashFileName}";
            (bool success, string text) = await LoadFileAsync(bbHashFilePath);
            if (!success)
            {
                Logger.Error(Label, $"Load {bbHashFilePath} failed!.");
                return false;
            }

            // If we didn't download the full content for this bb catalog, we must ignore the cache since it's might be outdated or corrupted.
            if (text != bbCatalogHash)
            {
                Logger.Debug(Label, $"Find {BuiltInCatalogHashFileName} at local but it's outdated or corrupted. cacheHash: {text}");
                return false;
            }

            return true;
        }

        private static async UniTask<(IResourceLocator, string)> LoadCatalogCacheAsync(string catalogName)
        {
            // Load cached catalog hash
            string ccHashFilePath = $"{CatalogCacheFolderPath}/{catalogName.Replace(".json", ".hash")}";
            (bool success, string ccHash) = await LoadFileAsync(ccHashFilePath);
            if (!success)
            {
                Logger.Error(Label, $"Load {ccHashFilePath} failed!.");
                return (null, null);
            }

            // Load cached catalog
            string catalogPath = $"{CatalogCacheFolderPath}/{catalogName}";
            (bool catalogSuccess, string catalogJson) = await LoadFileAsync(catalogPath);
            if (!catalogSuccess)
            {
                Logger.Error(Label, $"Load {catalogName} Failed.");
                return (null, null);
            }

            ContentCatalogData catalog;
            IResourceLocator locator;
            try
            {
                // Reference ContentCatalogProvider.OnCatalogLoaded()
                catalog = JsonUtility.FromJson<ContentCatalogData>(catalogJson);
                locator = catalog.CreateLocator();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
                return (null, null);
            }
            finally
            {

            }

            return (locator, ccHash);
        }

        private void ValidateIfCustomRemoteUrl(IResourceLocator catalog)
        {
            if (catalog.LocatorId != ResourceManagerRuntimeData.kCatalogAddress)
            {
                // This might be AddressableAssetSettings if using AssetDatabase mode.
                return;
            }

            // Check if has remote content
            IResourceLocation catalogLocation = GetCatalogLocation(catalog);
            if (catalogLocation.HasDependencies &&
                catalogLocation.Dependencies.Count == (int)ContentCatalogProvider.DependencyHashIndex.Count)
            {
                string remoteHashUrl = catalogLocation.Dependencies[(int)ContentCatalogProvider.DependencyHashIndex.Remote].InternalId;
                if (remoteHashUrl.StartsWith(UrlReplacement) || ResourceManagerConfig.IsPathRemote(remoteHashUrl))
                {
                    _hasRemoteContent = true;
                    _remoteHashUrl = remoteHashUrl;
                    _isCustomRemoteUrl = _remoteHashUrl.StartsWith(UrlReplacement);
                }
            }

            Logger.Debug(Label, $"HasRemoteContent: {_hasRemoteContent}");
            if (_hasRemoteContent)
            {
                Logger.Debug(Label, $"RemoteHashUrl: {_remoteHashUrl}");
            }
        }

        private static IResourceLocation GetCatalogLocation(IResourceLocator catalog)
        {
            // Addressables.Instance { get; }
            PropertyInfo instanceGetter = typeof(Addressables).GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Static);
            object addressablesImpl = instanceGetter.GetValue(null);

            // AddressablesImpl.GetLocatorInfo(string locatorId)
            MethodInfo getLocatorInfoMethod = addressablesImpl.GetType().GetMethod("GetLocatorInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            object locatorInfo = getLocatorInfoMethod.Invoke(addressablesImpl, new object[] { catalog.LocatorId });

            // LocatorInfo.CatalogLocation { get; }
            PropertyInfo catalogLocationGetter = locatorInfo.GetType().GetProperty("CatalogLocation");
            var catalogLocation = catalogLocationGetter.GetValue(locatorInfo) as IResourceLocation;
            return catalogLocation;
        }

        private static async UniTask<(bool, string)> LoadFileAsync(string path)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            const string FileSystemPrefix = "jar:file://";

            if (!path.StartsWith(FileSystemPrefix))
            {
                path = FileSystemPrefix + path;
            }

            UnityWebRequest request = UnityWebRequest.Get(path);
            try
            {
                await request.SendWebRequest();
            }
            catch (System.Exception)
            {
                return (false, null);
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Logger.Error(Label, $"LoadFileAsync failed at UnityWebRequest: {request.error}");
                return (false, null);
            }

            string text = request.downloadHandler.text;
            return (true, text);
#else
            if (!File.Exists(path))
            {
                return (false, null);
            }

            string text = File.ReadAllText(path);
            await UniTask.Yield();
            return (true, text);
#endif
        }

        private string TransformInternalId(IResourceLocation location)
        {
            // If we don't use remote content, we don't need to transform.
            if (!HasRemoteContent)
            {
                return location.InternalId;
            }

            // We handle for AssetBundle, catalog.hash and catalog.json
            if (location.ResourceType != typeof(IAssetBundleResource) &&
                location.ResourceType != typeof(IResourceLocator) &&
                location.ResourceType != typeof(ContentCatalogData) &&
                location.ResourceType != typeof(string))
            {
                return location.InternalId;
            }

            // Check if it's built-in content
            string fileName = Path.GetFileName(location.InternalId);
            if (_builtInBundleNames.Contains(fileName))
            {
                return $"{BuiltInBundleFolderPath}/{fileName}";
            }

            // If we don't use custom URL, we don't need to transform.
            if (!IsCustomRemoteUrl)
            {
                return location.InternalId;
            }

            // Check URL
            if (location.InternalId.StartsWith(UrlReplacement))
            {
                // No CDN setting, we can't do nothing.
                if (_cdnSetting == null || string.IsNullOrEmpty(_cdnSetting.urlFormat))
                {
                    Logger.Warn(Label, $"Use RemoteUrl but has no CDN setting, can't transform internal id. InternalId: {location.InternalId}");
                    return location.InternalId;
                }

                string customUrl = string.Format(_cdnSetting.urlFormat, fileName);
                return customUrl;
            }
            else
            {
                return location.InternalId;
            }
        }

        private void OverrideWebRequest(UnityWebRequest webRequest)
        {
            if (_cdnSetting != null && !string.IsNullOrEmpty(_cdnSetting.authorization))
            {
                webRequest.SetRequestHeader("Authorization", _cdnSetting.authorization);
            }
        }

        public void SetCdnSetting(CdnSetting setting)
        {
            _cdnSetting = setting;
        }

        private async UniTask<(bool, string)> DownloadTextFileAsync(string url)
        {
            var www = new UnityWebRequest(url)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };
            OverrideWebRequest(www);

            try
            {
                await www.SendWebRequest();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
                return (false, null);
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Logger.Error(www.error);
                return (false, null);
            }
            else
            {
                string text = www.downloadHandler.text;
                return (true, text);
            }
        }

        public async UniTask<CheckUpdateResult> CheckUpdateAsync()
        {
            // If we are using local bundles.
            if (!HasRemoteContent)
            {
                return CheckUpdateResult.EmptySuccess;
            }

            // Compare remote hash with cache hash
            string catalogHashName = Path.GetFileName(_remoteHashUrl);
            string remoteHashPath = IsCustomRemoteUrl ? string.Format(_cdnSetting.urlFormat, catalogHashName) : _remoteHashUrl;
            (bool hashSuccess, string remoteHash) = await DownloadTextFileAsync(remoteHashPath);
            if (!hashSuccess)
            {
                Logger.Error(Label, "CheckUpdateAsync Failed.");
                return CheckUpdateResult.Failure;
            }

            if (remoteHash == _mainCatalogHash)
            {
                Logger.Debug(Label, "No update found.");
                return CheckUpdateResult.EmptySuccess;
            }

            // Load catalog and collect keys
            string remoteCatalogPath = remoteHashPath.Replace(".hash", ".json");
            (bool catalogSuccess, string catalogJson) = await DownloadTextFileAsync(remoteCatalogPath);
            if (!catalogSuccess)
            {
                Logger.Error(Label, "CheckUpdateAsync Failed.");
                return CheckUpdateResult.Failure;
            }

            // Load catalog
            ContentCatalogData remoteCatalog;
            IResourceLocator locator;
            try
            {
                // Reference ContentCatalogProvider.OnCatalogLoaded()
                remoteCatalog = JsonUtility.FromJson<ContentCatalogData>(catalogJson);
                locator = remoteCatalog.CreateLocator();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
                return CheckUpdateResult.Failure;
            }

            // Note: Temporary replace the catalog locator, so Addressable can locate keys to latest resource.
            // We'll recover after this process.
            Addressables.RemoveResourceLocator(_mainCatalog);
            Addressables.AddResourceLocator(locator);

            // Get download size with keys
            long size = CalculateDownloadSize(locator, out List<string> needToUpdateKeys);
            Logger.Debug(Label, $"DownloadSize: {size} bytes");
            var sb = new System.Text.StringBuilder();
            foreach (string key in needToUpdateKeys)
            {
                sb.AppendLine(key);
            }
            Logger.Debug(Label, $"NeedUpdateKeys: {sb}");

            // Recover the catalog locator
            Addressables.RemoveResourceLocator(locator);
            Addressables.AddResourceLocator(_mainCatalog);

            return new CheckUpdateResult
            {
                success = true,
                size = size,
                remoteCatalog = remoteCatalog,
                remoteCatalogLocator = locator,
                remoteCatalogHash = remoteHash,
                needToUpdateKeys = needToUpdateKeys,
            };
        }

        // Reference AddressablesImpl.GetDownloadSizeAsync()
        private long CalculateDownloadSize(IResourceLocator locator, out List<string> needToUpdateKeys)
        {
            var allLocations = new List<IResourceLocation>();
            foreach (object key in locator.Keys)
            {
                IList<IResourceLocation> locations;
                if (key is IList<IResourceLocation>)
                {
                    locations = key as IList<IResourceLocation>;
                }
                else if (key is IResourceLocation)
                {
                    locations = new List<IResourceLocation>(1)
                    {
                        key as IResourceLocation
                    };
                }
                else if (!locator.Locate(key, typeof(object), out locations))
                {
                    Logger.Error(Label, $"Can't locate key: {key}");
                }

                for (var i = 0; i < locations.Count; i++)
                {
                    if (locations[i].HasDependencies)
                    {
                        allLocations.AddRange(locations[i].Dependencies);
                    }
                }
            }

            long totalSize = 0;
            var distinct = allLocations.Distinct();
            needToUpdateKeys = new List<string>(distinct.Count());
            foreach (IResourceLocation location in distinct)
            {
                if (location.Data is ILocationSizeData sizeData)
                {
                    long size = sizeData.ComputeSize(location, Addressables.ResourceManager);
                    if (size > 0)
                    {
                        totalSize += size;
                        needToUpdateKeys.Add(location.PrimaryKey);
                    }
                }
            }

            return totalSize;
        }

        public async UniTask<bool> DownloadUpdateAsync(CheckUpdateResult updateResult, System.Action<float> progressCallback, System.Action completeCallback)
        {
            // Note: Temporary replace the catalog locator, so Addressable can locate keys to latest resource.
            // We'll recover after this process.
            Addressables.RemoveResourceLocator(_mainCatalog);
            Addressables.AddResourceLocator(updateResult.remoteCatalogLocator);

            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(updateResult.needToUpdateKeys,
                Addressables.MergeMode.Union, false);
            float progress = 0f;

            while (downloadHandle.Status == AsyncOperationStatus.None)
            {
                float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
                if (percentageComplete > progress * 1.1f)  // Report at most every 10% or so
                {
                    progress = percentageComplete;
                    Logger.Debug(Label, $"Download Progress: {progress:F4}");
                    progressCallback?.Invoke(progress);
                }
                await UniTask.Yield();
            }

            // Recover the catalog locator
            Addressables.RemoveResourceLocator(updateResult.remoteCatalogLocator);
            Addressables.AddResourceLocator(_mainCatalog);

            bool success = downloadHandle.Status == AsyncOperationStatus.Succeeded;
            Addressables.Release(downloadHandle);
            if (!success)
            {
                Logger.Error(Label, "DownloadDependenciesAsync Failed.");
                return false;
            }

            Logger.Debug(Label, $"Download Succeed!");

            // Load this new catalog and hash
            if (!Directory.Exists(CatalogCacheFolderPath))
            {
                Directory.CreateDirectory(CatalogCacheFolderPath);
            }

            string catalogHashName = Path.GetFileName(_remoteHashUrl);
            string catalogName = catalogHashName.Replace(".hash", ".json");
            string bbHashFilePathInCC = $"{CatalogCacheFolderPath}/{BuiltInCatalogHashFileName}";
            string ccHashFilePath = $"{CatalogCacheFolderPath}/{catalogHashName}";
            string ccFilePath = $"{CatalogCacheFolderPath}/{catalogName}";
            await File.WriteAllTextAsync(bbHashFilePathInCC, _buildInCatalogHash);
            await File.WriteAllTextAsync(ccHashFilePath, updateResult.remoteCatalogHash);
            await File.WriteAllTextAsync(ccFilePath, JsonUtility.ToJson(updateResult.remoteCatalog));

            // Load from cache
            (IResourceLocator cachedCatalog, string cachedCatalogHash) = await LoadCatalogCacheAsync(catalogName);
            if (cachedCatalog != null)
            {
                Addressables.RemoveResourceLocator(_mainCatalog);
                Addressables.AddResourceLocator(cachedCatalog);
                _mainCatalog = cachedCatalog;
                _mainCatalogHash = cachedCatalogHash;
            }

            completeCallback?.Invoke();
            return true;
        }

        public async UniTask<List<string>> GetKeysByLabelAsync<T>(string label)
        {
            IList<IResourceLocation> locationList = await Addressables.LoadResourceLocationsAsync(label, typeof(T));
            if (locationList == null)
            {
                Logger.Error(Label, $"GetKeysByLabelAsync failed by unknown reason");
                return new List<string>();
            }

            var result = new List<string>(locationList.Count);
            for (var i = 0; i < locationList.Count; i++)
            {
                result.Add(locationList[i].PrimaryKey);
            }

            return result;
        }

        public T LoadAsset<T>(string assetKey) where T : Object
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                Logger.Error(Label, "AssetLoadFailed: Empty key");
                return null;
            }

            bool success = _assetRepository.TryGetAsset(assetKey, out T asset);
            if (success)
            {
                return asset;
            }
            else
            {
                asset = LoadAssetFromAddressables<T>(assetKey);
                return asset;
            }
        }

        private T LoadAssetFromAddressables<T>(string assetKey) where T : Object
        {
            IList<IResourceLocation> locationList = Addressables.LoadResourceLocationsAsync(assetKey, typeof(T)).WaitForCompletion();
            if (locationList == null || locationList.Count == 0)
            {
                Logger.Error(Label, $"AssetLoadFailed: Can't find asset with key '{assetKey}'");
                return null;
            }

            T asset;
            try
            {
                asset = Addressables.LoadAssetAsync<T>(locationList[0]).WaitForCompletion();
            }
            catch (System.Exception e)
            {
                asset = null;
                Logger.Fatal(e);
            }

            if (asset == null)
            {
                Logger.Error(Label, $"AssetLoadFailed: Can't load asset with key '{assetKey}'");
                return null;
            }

            _assetRepository.Add(assetKey, asset);
            return asset;
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetKey) where T : Object
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                Logger.Error(Label, "AssetLoadFailed: Empty key");
                return null;
            }

            {
                bool success = _assetRepository.TryGetAsset(assetKey, out T asset);
                if (success)
                {
                    return asset;
                }
            }

            if (_loadingTasks.ContainsKey(assetKey))
            {
                await WaitLoadingTaskComplete(assetKey);
                _ = _assetRepository.TryGetAsset(assetKey, out T asset);
                return asset;
            }
            else
            {
                UniTask<T> task = LoadAssetFromAddressablesAsync<T>(assetKey);

                T asset;
                _loadingTasks.Add(assetKey, task);
                try
                {
                    asset = await task;
                }
                finally
                {
                    _loadingTasks.Remove(assetKey);
                }

                return asset;
            }
        }

        private async UniTask WaitLoadingTaskComplete(string assetKey)
        {
            do
            {
                await UniTask.Yield();
            }
            while (_loadingTasks.ContainsKey(assetKey));
        }

        private async UniTask<T> LoadAssetFromAddressablesAsync<T>(string assetKey) where T : Object
        {
            IList<IResourceLocation> locationList = await Addressables.LoadResourceLocationsAsync(assetKey, typeof(T));
            if (locationList == null || locationList.Count == 0)
            {
                Logger.Error(Label, $"AssetLoadFailed: Can't find asset with key '{assetKey}'");
                return null;
            }

            T asset;
            try
            {
                asset = await Addressables.LoadAssetAsync<T>(locationList[0]);
            }
            catch (System.Exception e)
            {
                asset = null;
                Logger.Fatal(e);
            }

            if (asset == null)
            {
                Logger.Error(Label, $"AssetLoadFailed: Can't load asset with key '{assetKey}'");
                return null;
            }

            _assetRepository.Add(assetKey, asset);
            return asset;
        }

        public void UnloadAsset(string assetKey)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                Logger.Error(Label, "AssetUnloadFailed: Empty key");
                return;
            }

            bool success = _assetRepository.RemoveByKey(assetKey, out Object asset);
            if (!success)
            {
                Logger.Error(Label, $"AssetUnloadFailed: The asset with key '{assetKey}' is not in management");
                return;
            }

            Addressables.Release(asset);
        }

        public void UnloadAsset<T>(T asset) where T : Object
        {
            if (asset == null)
            {
                Logger.Error(Label, "AssetUnloadFailed: Asset is null");
                return;
            }

            bool success = _assetRepository.RemoveByAsset(asset, out _);
            if (!success)
            {
                Logger.Error(Label, $"AssetUnloadFailed: The asset '{asset.name}' is not in management");
                return;
            }

            Addressables.Release(asset);
        }

        public void UnloadAllAssets()
        {
            foreach (Object asset in _assetRepository.Assets)
            {
                if (asset != null)
                {
                    Addressables.Release(asset);
                }
            }

            _assetRepository.Clear();
        }

        public void LoadScene(string assetKey, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                Logger.Error(Label, "AssetUnloadFailed: Empty key");
                return;
            }

            bool success = _sceneRepository.TryGetValue(assetKey, out _);
            if (success)
            {
                Logger.Error(Label, $"SceneLoadFailed: The scene with key '{assetKey}' is already loaded");
                return;
            }

            LoadSceneFromAddressables(assetKey, mode);
        }

        private void LoadSceneFromAddressables(string assetKey, LoadSceneMode mode)
        {
            IList<IResourceLocation> locationList = Addressables.LoadResourceLocationsAsync(assetKey).WaitForCompletion();
            if (locationList == null || locationList.Count == 0)
            {
                Logger.Error(Label, $"SceneLoadFailed: Can't find scene with key {assetKey}");
                return;
            }

            SceneInstance sceneInstance = Addressables.LoadSceneAsync(assetKey, mode).WaitForCompletion();
            _sceneRepository.Add(assetKey, sceneInstance);
        }

        public async UniTask LoadSceneAsync(string assetKey, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                Logger.Error(Label, "AssetUnloadFailed: Empty key");
                return;
            }

            bool success = _sceneRepository.TryGetValue(assetKey, out _);
            if (success)
            {
                Logger.Error(Label, $"SceneLoadFailed: The scene with key '{assetKey}' is already loaded");
                return;
            }

            if (_haveSceneLoading)
            {
                Logger.Error(Label, "Have another scene loading. For API correctness, prohibit simultaneously loading multiple scenes. " +
                    "And actually we don't need to simultaneously load multiple scenes.");
                return;
            }

            _haveSceneLoading = true;
            try
            {
                await LoadSceneFromAddressablesAsync(assetKey, mode);
            }
            finally
            {
                _haveSceneLoading = false;
            }
        }

        private async UniTask LoadSceneFromAddressablesAsync(string assetKey, LoadSceneMode mode)
        {
            IList<IResourceLocation> locationList = await Addressables.LoadResourceLocationsAsync(assetKey);
            if (locationList == null || locationList.Count == 0)
            {
                Logger.Error(Label, $"SceneLoadFailed: Can't find scene with key {assetKey}");
                return;
            }

            SceneInstance sceneInstance = await Addressables.LoadSceneAsync(locationList[0], mode);
            _sceneRepository.Add(assetKey, sceneInstance);

            await UniTask.Yield();  // Fix the classic first frame bug (The scene has not awaked yet)
        }

        public void UnloadScene(string assetKey)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                Logger.Error(Label, "AssetUnloadFailed: Empty key");
                return;
            }

            bool success = _sceneRepository.TryGetValue(assetKey, out SceneInstance sceneInstance);
            if (!success)
            {
                Logger.Error(Label, $"SceneUnloadFailed: Scene with key {assetKey} is not in management");
                return;
            }

            _ = Addressables.UnloadSceneAsync(sceneInstance).WaitForCompletion();
            _sceneRepository.Remove(assetKey);
        }

        public async UniTask UnloadSceneAsync(string assetKey)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                Logger.Error(Label, "AssetUnloadFailed: Empty key");
                return;
            }

            bool success = _sceneRepository.TryGetValue(assetKey, out SceneInstance sceneInstance);
            if (!success)
            {
                Logger.Error(Label, $"SceneUnloadFailed: Scene with key {assetKey} is not in management");
                return;
            }

            _ = await Addressables.UnloadSceneAsync(sceneInstance);
            _sceneRepository.Remove(assetKey);
        }

        public async UniTask UnloadAllScenesAsync()
        {
            var tasks = new List<UniTask>(_sceneRepository.Count);
            foreach (SceneInstance sceneInstance in _sceneRepository.Values)
            {
                UniTask task = Addressables.UnloadSceneAsync(sceneInstance).ToUniTask();
                tasks.Add(task);
            }

            await UniTask.WhenAll(tasks);
            _sceneRepository.Clear();
        }

        public static void ClearCache()
        {
            // Delete all cache files in persistentDataPath
            string folder = $"{Application.persistentDataPath}/com.unity.addressables/";
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }

            // Delete cc folder
            if (Directory.Exists(CatalogCacheFolderPath))
            {
                Directory.Delete(CatalogCacheFolderPath, true);
            }

            // Clear bundle cache, might be failed if the cache is being used.
            bool success = Caching.ClearCache();
            if (success)
            {
                Logger.Info(Label, "Clear cache success.");
            }
            else
            {
                Logger.Error(Label, "Failed to clear cache, the cache might being used.");
            }
        }
    }
}
