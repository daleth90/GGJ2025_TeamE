using UnityEngine;

namespace Bubble
{
    public class DeadZone : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerStatus playerStatus = null)
        {
            GameSystemModel.instance.GameFailAction?.Invoke();
        }
    }
}
