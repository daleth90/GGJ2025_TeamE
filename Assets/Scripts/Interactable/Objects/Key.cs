using UnityEngine;

namespace Bubble
{
    public class Key : MonoBehaviour
    {
        [SerializeField]
        private int _target;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<IPlayerOxygenStatus>().oxygen += 1;
                // Play Sound Effect...
                Destroy(gameObject);
            }
        }
    }
}
