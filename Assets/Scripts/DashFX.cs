using Physalia;
using UnityEngine;

namespace Bubble
{
    public class DashFX
    {
        private IVfxManager _vfxManager;
        private ParticleSystem dashfx;

        public void Init(Transform parent)
        {
            _vfxManager ??= ServiceLocator.Resolve<IVfxManager>();
            if (dashfx == null)
            {
                dashfx = _vfxManager.Get("Dash FX").GetComponent<ParticleSystem>();
                dashfx.gameObject.SetActive(true);
                dashfx.transform.parent = parent;
                dashfx.transform.localPosition = Vector3.zero;
            }
        }

        public void Play()
        {
            dashfx.Play();
        }

        public void OnDestory()
        {
            if (dashfx != null)
            {
                _vfxManager.Release(dashfx.gameObject);
                dashfx = null;
            }
        }
    }
}
