using Physalia;
using UnityEngine;

namespace Bubble
{
    public class Vine : MonoBehaviour
    {
        private IAudioManager _audioManager;

        private void Awake()
        {
            _audioManager = ServiceLocator.Resolve<IAudioManager>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                _audioManager.PlaySound("SFX_DashVine");
            }
        }
    }
}
