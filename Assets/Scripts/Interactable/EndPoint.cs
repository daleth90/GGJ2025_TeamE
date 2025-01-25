using UnityEngine;

namespace Bubble
{
    public class EndPoint : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            GameSystemModel.instance.GameSuccessfulAction?.Invoke();
        }
    }
}
