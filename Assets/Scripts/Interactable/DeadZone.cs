using Physalia;
using UnityEngine;

namespace Bubble
{
    public class DeadZone : MonoBehaviour, IInteractable
    {
        private IVfxManager _ivfxManager;

        private void Awake()
        {
            _ivfxManager = ServiceLocator.Resolve<IVfxManager>();
        }

        public void Interact(PlayerStatus playerStatus = null)
        {
            playerStatus.IsDeath = true;
            _ivfxManager.PlayOneShot("CFXR3 Hit Misc A", playerStatus.transform.position);
        }
    }
}
