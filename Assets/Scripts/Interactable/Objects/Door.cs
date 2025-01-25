using Interactable.Interfaces;
using UnityEngine;

namespace Interactable.Objects
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