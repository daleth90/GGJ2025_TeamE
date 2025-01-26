using Physalia;
using UnityEngine;

namespace Bubble
{
    public class UpFX
    {
        private IVfxManager _vfxManager;
        private ParticleSystem upfx;
        private bool isPlay;

        public void Init(Transform parent)
        {
            _vfxManager ??= ServiceLocator.Resolve<IVfxManager>();
            if (upfx == null)
            {
                upfx = _vfxManager.Get("CFXR4 Bubbles Breath Underwater Loop Variant").GetComponent<ParticleSystem>();
                upfx.gameObject.SetActive(true);
                upfx.transform.parent = parent;
                upfx.transform.localPosition = Vector3.zero;
            }
        }

        public void Play()
        {
            if (isPlay) return;
            isPlay = true;

            upfx.Play();
        }

        public void Stop()
        {
            if (!isPlay) return;
            isPlay = false;

            upfx.Stop();
        }

        public void OnDestory()
        {
            if (upfx != null)
            {
                _vfxManager.Release(upfx.gameObject);
                upfx = null;
            }
        }
    }
}
