using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Physalia
{
    public interface IVfxManager
    {
        UniTask PreloadVfxAsync(string key);

        bool CreatePool(string key, int size);
        UniTask<bool> CreatePoolAsync(string key, int size);
        void DestroyPool(string key);
        void DestroyAllPools();

        GameObject Get(string key);
        void Release(GameObject instance);
        void ReleaseAll(string key);

        void PlayOneShot(string key);
        void PlayOneShot(string key, float time);
        void PlayOneShot(string key, Vector3 worldPosition);
        void PlayOneShot(string key, Vector3 worldPosition, float time);
        void PlayOneShot(string key, Vector3 worldPosition, Quaternion worldRotation);
        void PlayOneShot(string key, Vector3 worldPosition, Quaternion worldRotation, float time);
        void PlayOneShot(string key, Transform parent);
        void PlayOneShot(string key, Transform parent, Quaternion localRotation);
        void PlayOneShot(string key, Transform parent, float time);
        void PlayOneShot(string key, Transform parent, Vector3 localPosition);
        void PlayOneShot(string key, Transform parent, Vector3 localPosition, float time);
        void PlayOneShot(string key, Transform parent, Vector3 localPosition, Quaternion localRotation);
        void PlayOneShot(string key, Transform parent, Vector3 localPosition, Quaternion localRotation, float time);
    }
}
