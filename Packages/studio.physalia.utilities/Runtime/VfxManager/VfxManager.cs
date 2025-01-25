using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Physalia
{
    public class VfxManager : IVfxManager
    {
        private static readonly Logger.Label Label = Logger.Label.CreateFromCurrentClass();
        private static readonly float DefaultOneShotTime = 3f;
        private const int MinDefaultPoolSize = 1;
        private const int MaxDefaultPoolSize = 64;

        private readonly IAssetManager _assetManager;
        private readonly VfxOneShotReleaser _oneShotReleaser;

        private readonly Dictionary<string, GameObjectPool> _poolTable = new(32);
        private readonly Dictionary<GameObject, GameObjectPool> _instanceToPoolTable = new(128);
        private readonly List<GameObject> _removeListCache = new(128);
        private int _defaultPoolSize = 10;

        public int DefaultPoolSize
        {
            get
            {
                return _defaultPoolSize;
            }
            set
            {
                if (value < MinDefaultPoolSize || value > MaxDefaultPoolSize)
                {
                    Logger.Warn(Label, $"DefaultSize={value} is invalid, should be in {MaxDefaultPoolSize}~{MaxDefaultPoolSize}. Already clammped.");
                    _defaultPoolSize = Mathf.Clamp(value, MinDefaultPoolSize, MaxDefaultPoolSize);
                }
                else
                {
                    _defaultPoolSize = value;
                }
            }
        }

        public VfxManager(IAssetManager assetManager)
        {
            _assetManager = assetManager;
            _oneShotReleaser = new GameObject(nameof(VfxOneShotReleaser)).AddComponent<VfxOneShotReleaser>();
            _oneShotReleaser.Construct(this);
        }

        public async UniTask PreloadVfxAsync(string key)
        {
            var prefab = await _assetManager.LoadAssetAsync<GameObject>(key);
            if (prefab == null)
            {
                Logger.Error(Label, $"Load vfx prefab failed: '{key}'");
            }
        }

        public bool CreatePool(string key, int size)
        {
            if (string.IsNullOrEmpty(key))
            {
                Logger.Warn(Label, $"Create VFX pool failed! The key is null or empty string.");
                return false;
            }

            if (_poolTable.ContainsKey(key))
            {
                Logger.Warn(Label, $"VFX pool of '{key}' has already existed. Skip creation!");
                return false;
            }

            var prefab = _assetManager.LoadAsset<GameObject>(key);
            if (prefab == null)
            {
                Logger.Error(Label, $"Load vfx prefab failed: '{key}'");
                return false;
            }

            var newPool = new GameObjectPool(prefab, size);
            _poolTable.Add(key, newPool);
            return true;
        }

        public async UniTask<bool> CreatePoolAsync(string key, int size)
        {
            if (string.IsNullOrEmpty(key))
            {
                Logger.Warn(Label, $"Create VFX pool failed! The key is null or empty string.");
                return false;
            }

            if (_poolTable.ContainsKey(key))
            {
                Logger.Warn(Label, $"VFX pool of '{key}' has already existed. Skip creation!");
                return false;
            }

            var prefab = await _assetManager.LoadAssetAsync<GameObject>(key);
            if (prefab == null)
            {
                Logger.Error(Label, $"Load vfx prefab failed: '{key}'");
                return false;
            }

            var newPool = new GameObjectPool(prefab, size);
            _poolTable.Add(key, newPool);
            return true;
        }

        public void DestroyPool(string key)
        {
            ReleaseAll(key);

            bool success = _poolTable.TryGetValue(key, out GameObjectPool pool);
            if (!success)
            {
                return;
            }

            pool.Destroy();
            _poolTable.Remove(key);
        }

        public void DestroyAllPools()
        {
            foreach (GameObjectPool pool in _poolTable.Values)
            {
                pool.Destroy();
            }

            _poolTable.Clear();
            _instanceToPoolTable.Clear();
            _oneShotReleaser.Clear();
        }

        public GameObject Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Logger.Warn(Label, $"Get VFX failed! The key is null or empty string. Return null.");
                return null;
            }

            bool success = _poolTable.TryGetValue(key, out GameObjectPool pool);
            if (success)
            {
                GameObject instance = pool.Get();
                _instanceToPoolTable.Add(instance, pool);
                return instance;
            }

            Logger.Warn(Label, $"Pool not found: '{key}'. Create new pool immediately!" +
                $"Note the pool should be manually created, this action and log hurt performance.");
            success = CreatePool(key, DefaultPoolSize);
            if (!success)
            {
                return null;
            }

            return Get(key);
        }

        public void Release(GameObject instance)
        {
            bool success = _instanceToPoolTable.TryGetValue(instance, out GameObjectPool pool);
            if (success)
            {
                _ = _instanceToPoolTable.Remove(instance);
                _ = _oneShotReleaser.Remove(instance);
                pool.Release(instance);
                return;
            }

            Logger.Error(Label, $"Releasing '{instance.name}' not belong to any pool. Destroyed!");
            Object.Destroy(instance);
            return;
        }

        public void ReleaseAll(string key)
        {
            bool success = _poolTable.TryGetValue(key, out GameObjectPool pool);
            if (success)
            {
                pool.ReleaseAll();

                // Remove all instance caches belong to this pool
                foreach (KeyValuePair<GameObject, GameObjectPool> pair in _instanceToPoolTable)
                {
                    if (pair.Value == pool)
                    {
                        _removeListCache.Add(pair.Key);
                    }
                }

                for (var i = 0; i < _removeListCache.Count; i++)
                {
                    GameObject instance = _removeListCache[i];
                    _ = _instanceToPoolTable.Remove(instance);
                    _ = _oneShotReleaser.Remove(instance);
                }
                _removeListCache.Clear();
            }
        }

        public void PlayOneShot(string key)
        {
            PlayOneShot(key, Vector3.zero, Quaternion.identity, DefaultOneShotTime);
        }

        public void PlayOneShot(string key, float time)
        {
            PlayOneShot(key, Vector3.zero, Quaternion.identity, time);
        }

        public void PlayOneShot(string key, Vector3 worldPosition)
        {
            PlayOneShot(key, worldPosition, Quaternion.identity, DefaultOneShotTime);
        }

        public void PlayOneShot(string key, Vector3 worldPosition, float time)
        {
            PlayOneShot(key, worldPosition, Quaternion.identity, time);
        }

        public void PlayOneShot(string key, Vector3 worldPosition, Quaternion worldRotation)
        {
            PlayOneShot(key, worldPosition, worldRotation, DefaultOneShotTime);
        }

        public void PlayOneShot(string key, Vector3 worldPosition, Quaternion worldRotation, float time)
        {
            if (string.IsNullOrEmpty(key))
            {
                Logger.Warn(Label, $"PlayOneShot failed! The key is null or empty string.");
                return;
            }

            GameObject instance = Get(key);
            if (instance == null)
            {
                return;
            }

            instance.transform.SetPositionAndRotation(worldPosition, worldRotation);
            instance.SetActive(true);

            _oneShotReleaser.Add(instance, time);
        }

        public void PlayOneShot(string key, Transform parent)
        {
            PlayOneShot(key, parent, Vector3.zero, DefaultOneShotTime);
        }
        public void PlayOneShot(string key, Transform parent, Quaternion localRotation)
        {
            PlayOneShot(key, parent, Vector3.zero, localRotation, DefaultOneShotTime);
        }
        public void PlayOneShot(string key, Transform parent, float time)
        {
            PlayOneShot(key, parent, Vector3.zero, time);
        }

        public void PlayOneShot(string key, Transform parent, Vector3 localPosition)
        {
            PlayOneShot(key, parent, localPosition, Quaternion.identity, DefaultOneShotTime);
        }

        public void PlayOneShot(string key, Transform parent, Vector3 localPosition, float time)
        {
            PlayOneShot(key, parent, localPosition, Quaternion.identity, time);
        }

        public void PlayOneShot(string key, Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            PlayOneShot(key, parent, localPosition, localRotation, DefaultOneShotTime);
        }

        public void PlayOneShot(string key, Transform parent, Vector3 localPosition, Quaternion localRotation, float time)
        {
            if (string.IsNullOrEmpty(key))
            {
                Logger.Warn(Label, $"PlayOneShot failed! The key is null or empty string.");
                return;
            }

            GameObject instance = Get(key);
            if (instance == null)
            {
                return;
            }

            if (instance.transform is RectTransform rectTransform)
            {
                rectTransform.SetParent(parent, false);
                rectTransform.anchoredPosition = localPosition;
                rectTransform.rotation = localRotation;
            }
            else
            {
                instance.transform.SetParent(parent, false);
                instance.transform.SetLocalPositionAndRotation(localPosition, localRotation);
            }
            instance.SetActive(true);

            _oneShotReleaser.Add(instance, time);
        }
    }
}
