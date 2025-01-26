using Physalia;
using UnityEngine;

namespace Bubble
{
    public class UpFX
    {
        private IVfxManager _ivfxManager;
        private GameObject fxObject;

        public void Play(Transform transform)
        {
            Init();

            fxObject.transform.parent = transform;
            fxObject.transform.localPosition = Vector3.zero;
        }

        public void Stop()
        {
            Init();
            _ivfxManager.Release(fxObject);
        }

        private void Init()
        {
            if (_ivfxManager == null) _ivfxManager = ServiceLocator.Resolve<IVfxManager>();

            if (fxObject == null)
            {
                fxObject = _ivfxManager.Get("CFXR4 Bubbles Breath Underwater Loop Variant");
            }
        }
    }
}
