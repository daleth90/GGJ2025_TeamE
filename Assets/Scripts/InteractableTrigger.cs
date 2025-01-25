using UnityEngine;
namespace Bubble
{
    public class InteractableTrigger : MonoBehaviour
    {
        [SerializeField] private LayerMask target_layerMask;
        [SerializeField] private PlayerStatus playerStatus;
        private int target_layer;

        private void Start()
        {
            target_layer = (int)Mathf.Log(target_layerMask.value, 2f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != target_layer) return;
            IInteractable interactable = other.GetComponent<IInteractable>();
            interactable?.Interact(playerStatus);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer != target_layer) return;
            IInteractable interactable = other.GetComponent<IInteractable>();
            interactable?.CancelInteract();
        }
    }
}

