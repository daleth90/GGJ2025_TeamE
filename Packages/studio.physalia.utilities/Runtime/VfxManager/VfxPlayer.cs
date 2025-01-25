using System.Collections.Generic;
using UnityEngine;

namespace Physalia
{
    public sealed class VfxPlayer : MonoBehaviour
    {
        private IVfxManager _vfxManager;

        private readonly Dictionary<string, GameObject> _usingVfx = new(4);

        private void Awake()
        {
            _vfxManager = ServiceLocator.Resolve<IVfxManager>();
        }

        private void OnDestroy()
        {
            RemoveAllVfxLoop();
        }

        public void PlayVfx(string key, Vector3 worldPosition)
        {
            _vfxManager.PlayOneShot(key, worldPosition, Quaternion.identity);
        }

        public void PlayVfx(string key, Vector3 worldPosition, float duration)
        {
            _vfxManager.PlayOneShot(key, worldPosition, Quaternion.identity, duration);
        }

        public void PlayVfx(string key, Vector3 worldPosition, Quaternion worldRotation)
        {
            _vfxManager.PlayOneShot(key, worldPosition, worldRotation);
        }

        public void PlayVfx(string key, Vector3 worldPosition, Quaternion worldRotation, float duration)
        {
            _vfxManager.PlayOneShot(key, worldPosition, worldRotation, duration);
        }

        public void PlayVfx(string key, Transform parent)
        {
            _vfxManager.PlayOneShot(key, parent, Vector3.zero, Quaternion.identity);
        }

        public void PlayVfx(string key, Transform parent, float duration)
        {
            _vfxManager.PlayOneShot(key, parent, Vector3.zero, Quaternion.identity, duration);
        }

        public void PlayVfx(string key, Transform parent, Vector3 localPosition)
        {
            _vfxManager.PlayOneShot(key, parent, localPosition, Quaternion.identity);
        }

        public void PlayVfx(string key, Transform parent, Vector3 localPosition, float duration)
        {
            _vfxManager.PlayOneShot(key, parent, localPosition, Quaternion.identity, duration);
        }

        public void PlayVfx(string key, Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            _vfxManager.PlayOneShot(key, parent, localPosition, localRotation);
        }

        public void PlayVfx(string key, Transform parent, Vector3 localPosition, Quaternion localRotation, float duration)
        {
            _vfxManager.PlayOneShot(key, parent, localPosition, localRotation, duration);
        }

        public bool HasVfxLoop(string key)
        {
            return _usingVfx.ContainsKey(key);
        }

        public void AddVfxLoop(string key, Transform parent)
        {
            AddVfxLoop(key, parent, Vector2.zero);
        }

        public void AddVfxLoop(string key, Transform parent, Vector2 offset)
        {
            GameObject vfx = _vfxManager.Get(key);
            if (vfx != null)
            {
                _usingVfx.Add(key, vfx);
                vfx.transform.SetParent(parent, false);
                vfx.transform.localPosition = offset;
                vfx.SetActive(true);
            }
        }

        public void RemoveVfxLoop(string key)
        {
            if (_usingVfx.TryGetValue(key, out GameObject vfx))
            {
                _vfxManager.Release(vfx);
                _usingVfx.Remove(key);
            }
        }

        public void RemoveAllVfxLoop()
        {
            foreach (GameObject vfx in _usingVfx.Values)
            {
                _vfxManager.Release(vfx);
            }
            _usingVfx.Clear();
        }
    }
}
