using Physalia;
using UnityEngine;

namespace Bubble
{
    public class UpFX
    {
        private IVfxManager _vfxManager;
        private GameObject fxObject;

        private void Init()
        {
            _vfxManager ??= ServiceLocator.Resolve<IVfxManager>();
        }

        public void Play(Transform parent)
        {
            Init();
            if (fxObject == null)
            {
                fxObject = _vfxManager.Get("CFXR4 Bubbles Breath Underwater Loop Variant");
                fxObject.transform.parent = parent;
                fxObject.transform.localPosition = Vector3.zero;
                fxObject.SetActive(true);
            }
        }

        public void Stop()
        {
            Init();
            if (fxObject != null)
            {
                _vfxManager.Release(fxObject);
                fxObject = null;
            }
        }
    }
}
