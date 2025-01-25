using UnityEngine;

namespace Bubble
{
    public class EndPoint : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerStatus playerStatus = null)
        {
            GameSystemModel.instance.GameSuccessfulAction?.Invoke();
        }
    }
}
