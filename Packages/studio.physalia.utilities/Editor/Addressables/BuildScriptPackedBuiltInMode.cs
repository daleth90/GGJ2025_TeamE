using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace Physalia.AssetManager
{
    [CreateAssetMenu(fileName = "BuildScriptPackedBuiltInMode.asset", menuName = "Addressables/Content Builders/Build Script With BuiltInBundles")]
    public class BuildScriptPackedBuiltInMode : BuildScriptPackedMode
    {
        private List<string> _builtInBundles;

        public override string Name => "Build Script With BuiltInBundles";

        // Just for initialization. This seems like the best start point we can re-initialize the list.
        protected override string ProcessAllGroups(AddressableAssetsBuildContext aaContext)
        {
            _builtInBundles = new List<string>();
            return base.ProcessAllGroups(aaContext);
        }

        // Collect the output bundle names after constructing.
        protected override string ConstructAssetBundleName(AddressableAssetGroup assetGroup,
            BundledAssetGroupSchema schema, BundleDetails info, string assetBundleName)
        {
            string result = base.ConstructAssetBundleName(assetGroup, schema, info, assetBundleName);
            if (assetGroup != null && assetGroup.HasSchema<PlayerBuiltInSchema>())
            {
                _builtInBundles.Add(result);
            }
            return result;
        }

        protected override TResult DoBuild<TResult>(AddressablesDataBuilderInput builderInput, AddressableAssetsBuildContext aaContext)
        {
            TResult result = base.DoBuild<TResult>(builderInput, aaContext);
            if (result != null)  // Success if has result
            {
                try
                {
                    CopyRemoteBundlesToStreamingAssets(aaContext, _builtInBundles);
                    WriteBuiltInBundleNames(_builtInBundles);
                    CopyCatalogHashFile(builderInput, aaContext);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            return result;
        }

        private void CopyRemoteBundlesToStreamingAssets(AddressableAssetsBuildContext aaContext, List<string> builtInBundles)
        {
            string remoteBuildPath = GetRemoteBuildPath(aaContext.Settings);
            string builtInBundlePath = AddressableAssetManager.BuiltInBundleFolderPath;
            if (!Directory.Exists(builtInBundlePath))
            {
                Directory.CreateDirectory(builtInBundlePath);
            }

            for (var i = 0; i < builtInBundles.Count; i++)
            {
                string bundleName = builtInBundles[i];
                string bundlePath = Path.Combine(remoteBuildPath, bundleName);
                string destinationPath = Path.Combine(builtInBundlePath, bundleName);
                File.Copy(bundlePath, destinationPath, true);
            }
        }

        private static string GetLocalBuildPath(AddressableAssetSettings settings)
        {
            AddressableAssetProfileSettings profileSettings = settings.profileSettings;
            string value = profileSettings.GetValueByName(settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath);
            string result = profileSettings.EvaluateString(settings.activeProfileId, value);
            return result;
        }

        private static string GetRemoteBuildPath(AddressableAssetSettings settings)
        {
            AddressableAssetProfileSettings profileSettings = settings.profileSettings;
            string value = profileSettings.GetValueByName(settings.activeProfileId, AddressableAssetSettings.kRemoteBuildPath);
            string result = profileSettings.EvaluateString(settings.activeProfileId, value);
            return result;
        }

        private void WriteBuiltInBundleNames(List<string> builtInBundles)
        {
            string builtInBundleListPath = AddressableAssetManager.BuiltInBundleListPath;
            string json = JsonConvert.SerializeObject(builtInBundles);
            File.WriteAllText(builtInBundleListPath, json);
        }

        private void CopyCatalogHashFile(AddressablesDataBuilderInput builderInput, AddressableAssetsBuildContext aaContext)
        {
            // Get catalog hash from the hash file
            // Note: These lines are copied from BuildScriptPackedMode.CreateRemoteCatalog()
            AddressableAssetSettings aaSettings = aaContext.Settings;
            string versionedFileName =
                aaSettings.profileSettings.EvaluateString(aaSettings.activeProfileId, $"catalog_{builderInput.PlayerVersion}");
            string remoteBuildFolder = aaSettings.RemoteCatalogBuildPath.GetValue(aaSettings);
            var remoteHashBuildPath = $"{remoteBuildFolder}/{versionedFileName}.hash";

            // Copy the hash file to built-in path
            string builtInCatalogHashPath = AddressableAssetManager.BuiltInCatalogHashPath;
            File.Copy(remoteHashBuildPath, builtInCatalogHashPath, true);
        }

        public override void ClearCachedData()
        {
            // Note: The base method only clear data in 'Library/com.unity.addressables/aa/<Platform>'
            base.ClearCachedData();

            // Delete the actual build path
            string localBuildPath = GetLocalBuildPath(AddressableAssetSettingsDefaultObject.Settings);
            if (Directory.Exists(localBuildPath))
            {
                Directory.Delete(localBuildPath, true);
            }

            string remoteBuildPath = GetRemoteBuildPath(AddressableAssetSettingsDefaultObject.Settings);
            if (Directory.Exists(remoteBuildPath))
            {
                Directory.Delete(remoteBuildPath, true);
            }

            // Delete built-in bundles
            string builtInBundlePath = AddressableAssetManager.BuiltInBundleFolderPath;
            if (Directory.Exists(builtInBundlePath))
            {
                Directory.Delete(builtInBundlePath, true);
                File.Delete($"{builtInBundlePath}.meta");
            }
        }
    }
}
