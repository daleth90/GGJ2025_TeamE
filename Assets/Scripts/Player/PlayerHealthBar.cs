using UnityEngine;
using UnityEngine.UI;

namespace Bubble
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField]
        private PlayerStatus _playerStatus;
        [SerializeField]
        private SlicedFilledImage _healthBarFill;

        private void Update()
        {
            _healthBarFill.fillAmount = _playerStatus.oxygen / (float)_playerStatus.MaxOxygen;
        }
    }
}
