using System;
using TMPro;
using UnityEditor;
using UnityEngine;

// Reference: https://forum.unity.com/threads/tmpro-dynamic-font-asset-constantly-changes-in-source-control.1227831/#post-8935509
internal class TMProDynamicFontCleaner : AssetModificationProcessor
{
    private static string[] OnWillSaveAssets(string[] paths)
    {
        for (var i = 0; i < paths.Length; i++)
        {
            string path = paths[i];

            try
            {
                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);

                // GetMainAssetTypeAtPath() sometimes returns null. For example, when path leads to .meta file.
                if (assetType == null)
                {
                    continue;
                }

                // TMP_FontAsset is not marked as sealed class, so also checking for subclasses just in case.
                if (assetType != typeof(TMP_FontAsset) && !assetType.IsSubclassOf(typeof(TMP_FontAsset)))
                {
                    continue;
                }

                // Loading the asset only when we sure it is a font asset.
                var fontAsset = AssetDatabase.LoadMainAssetAtPath(path) as TMP_FontAsset;

                // Theoretically this case is not possible due to asset type check above, but to be on the safe side check for null.
                if (fontAsset == null)
                {
                    continue;
                }

                if (fontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic)
                {
                    continue;
                }

                fontAsset.ClearFontAssetData(true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Something went wrong while clearing dynamic font data. For more info look at previous log message. Font asset path: '{path}'");
            }
        }

        return paths;
    }
}
