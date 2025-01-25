using UnityEngine;

namespace Bubble
{
    public class Vine : MonoBehaviour
    {
        private Collider2D _collider;

        private void Start()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                _collider.enabled = !collision.collider.GetComponent<PlayerStatus>().isInDash;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                _collider.enabled = true;
            }
        }
    }
}