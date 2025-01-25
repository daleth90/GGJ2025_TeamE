using UnityEngine;

namespace Bubble
{
    public class OxygenCylinder : MonoBehaviour
    {
        [SerializeField]
        private int _recoveryAmount;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<IPlayerOxygenStatus>().oxygen += _recoveryAmount;
                // Play Sound Effect...
                Destroy(gameObject);
            }
        }
    }
}