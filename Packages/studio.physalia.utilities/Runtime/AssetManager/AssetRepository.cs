using System.Collections.Generic;
using UnityEngine;

namespace Physalia
{
    public class AssetRepository
    {
        private const int DefaultCapacity = 128;

        private static readonly Logger.Label Label = Logger.Label.CreateFromCurrentClass();

        private readonly Dictionary<string, Object> _keyToAssetCache = new(DefaultCapacity);
        private readonly Dictionary<Object, string> _assetToKeyCache = new(DefaultCapacity);

        public IEnumerable<string> Keys => _keyToAssetCache.Keys;
        public IEnumerable<Object> Assets => _keyToAssetCache.Values;

        public bool TryGetAsset<T>(string key, out T asset) where T : Object
        {
            bool success = _keyToAssetCache.TryGetValue(key, out Object @object);
            if (success)
            {
                if (@object is T)
                {
                    asset = @object as T;
                    return true;
                }
                else
                {
                    Logger.Warn(Label, $"GetAsset<{typeof(T).Name}>({key}) Failed! The key exists, but the type is '{@object.GetType().Name}'");
                    asset = null;
                    return false;
                }
            }
            else
            {
                asset = null;
                return false;
            }
        }

        public void Add(string key, Object asset)
        {
            _keyToAssetCache.Add(key, asset);
            _assetToKeyCache.Add(asset, key);
        }

        public bool RemoveByKey(string key, out Object asset)
        {
            bool success = _keyToAssetCache.Remove(key, out asset);
            if (!success)
            {
                return false;
            }

            _assetToKeyCache.Remove(asset);
            return true;
        }

        public bool RemoveByAsset(Object asset, out string key)
        {
            bool success = _assetToKeyCache.Remove(asset, out key);
            if (!success)
            {
                return false;
            }

            _keyToAssetCache.Remove(key);
            return true;
        }

        public void Clear()
        {
            _keyToAssetCache.Clear();
            _assetToKeyCache.Clear();
        }
    }
}
