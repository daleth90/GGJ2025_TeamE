using UnityEngine;

namespace Bubble
{
    public class DeadZone : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            GameSystemModel.instance.GameFailAction?.Invoke();
        }
    }
}
