using UnityEngine;

namespace Bubble
{
    public class Trap : MonoBehaviour
    {
        [SerializeField]
        private int _damage;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<IPlayerOxygenStatus>().oxygen -= _damage;
            }
        }
    }
}
