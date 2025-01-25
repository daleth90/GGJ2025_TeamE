using Physalia;
using UnityEngine;

namespace Bubble
{
    public class OxygenCylinder : MonoBehaviour
    {
        [SerializeField]
        private int _recoveryAmount;

        private IAudioManager _audioManager;

        private void Awake()
        {
            _audioManager = ServiceLocator.Resolve<IAudioManager>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerStatus>().oxygen += _recoveryAmount;
                _audioManager.PlaySound("SFX_AirBump");
                gameObject.SetActive(false);
            }
        }
    }
}
