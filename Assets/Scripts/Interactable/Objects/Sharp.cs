using UnityEngine;

namespace Bubble
{
    public class Sharp : MonoBehaviour
    {
        [SerializeField]
        private int _damage;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<IPlayerStatus>().oxygen -= _damage;
            }
        }
    }
}