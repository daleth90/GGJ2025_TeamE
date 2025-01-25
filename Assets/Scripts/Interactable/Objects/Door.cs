using UnityEngine;

namespace Bubble
{
    public class Door : MonoBehaviour, IKeyInteractable
    {
        public void Interact()
        {
            // Play Animation
            // Play Sound Effect
            Destroy(gameObject);
        }
    }
}