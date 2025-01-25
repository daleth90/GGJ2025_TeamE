using System.Collections.Generic;
using UnityEngine;

namespace Physalia
{
    internal class VfxOneShotReleaser : MonoBehaviour
    {
        private IVfxManager _vfxManager;

        private readonly Dictionary<GameObject, float> _oneShotTimeTable = new(128);
        private readonly List<GameObject> _instances = new(128);

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Construct(IVfxManager vfxManager)
        {
            _vfxManager = vfxManager;
        }

        public void Add(GameObject instance, float time)
        {
            _oneShotTimeTable.Add(instance, time);
            _instances.Add(instance);
        }

        public bool Remove(GameObject instance)
        {
            bool success = _oneShotTimeTable.Remove(instance);
            if (success)
            {
                _ = _instances.Remove(instance);
            }

            return success;
        }

        public void Clear()
        {
            _oneShotTimeTable.Clear();
            _instances.Clear();
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            for (var i = _instances.Count - 1; i >= 0; i--)
            {
                GameObject instance = _instances[i];
                float remainTime = _oneShotTimeTable[instance];
                remainTime -= deltaTime;
                _oneShotTimeTable[instance] = remainTime;

                if (remainTime <= 0f)
                {
                    // Note: Though Release will call this Remove method internally, we should remove by ourselves because removing by index is faster.
                    _oneShotTimeTable.Remove(instance);
                    _instances.RemoveAt(i);
                    _vfxManager.Release(instance);
                }
            }
        }

        // Note: Release all one-shot instances when this object is destroyed.
        private void OnDestroy()
        {
            // Release all one-shot instances
            for (var i = _instances.Count - 1; i >= 0; i--)
            {
                // Note: Though Release will call this Remove method internally, we should remove by ourselves because removing by index is faster.
                GameObject instance = _instances[i];
                _oneShotTimeTable.Remove(instance);
                _instances.RemoveAt(i);
                _vfxManager.Release(instance);
            }
        }
    }
}
