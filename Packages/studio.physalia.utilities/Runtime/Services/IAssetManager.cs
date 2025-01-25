using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Physalia
{
    public interface IAssetManager
    {
        public T LoadAsset<T>(string assetKey) where T : Object;
        public UniTask<T> LoadAssetAsync<T>(string assetKey) where T : Object;
        public void UnloadAsset(string assetKey);
        public void UnloadAsset<T>(T asset) where T : Object;
        public void LoadScene(string assetKey, LoadSceneMode mode = LoadSceneMode.Single);
        public UniTask LoadSceneAsync(string assetKey, LoadSceneMode mode = LoadSceneMode.Single);
        public void UnloadScene(string assetKey);
        public UniTask UnloadSceneAsync(string assetKey);
    }
}
