using UnityEngine;
using UnityEngine.UI;

namespace Bubble
{
    public class EndUI : MonoBehaviour
    {
        [field: SerializeField] public Button restartButton { get; private set; }
        [field: SerializeField] public Button nextButton { get; private set; }
        [field: SerializeField] public Button quitButton { get; private set; }

        [SerializeField]
        private GameObject ui;
        [SerializeField]
        private Image _resultImage;
        [SerializeField]
        private Sprite _successSprite;
        [SerializeField]
        private Sprite _failSprite;

        public void Show(bool isWin)
        {
            _resultImage.sprite = isWin ? _successSprite : _failSprite;
            ui.SetActive(true);
        }

        public void Hide()
        {
            ui.SetActive(false);
        }
    }
}
