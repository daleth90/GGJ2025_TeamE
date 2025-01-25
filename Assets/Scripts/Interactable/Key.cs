using UnityEngine;

namespace Bubble
{
    public class Key : Consumable
    {
        [SerializeField]
        private KeyInteractable _target;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                _target.InteractWithKey();
                // Play Sound Effect...
                gameObject.SetActive(false);
            }
        }
    }
}
