using Cysharp.Threading.Tasks;
using Physalia;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bubble
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;

        private bool isStartClicked = false;

        private IAudioManager _audioManager;

        private void Awake()
        {
            _audioManager = ServiceLocator.Resolve<IAudioManager>();
            _startButton.onClick.AddListener(() => OnStartClicked().Forget());
        }

        private void Start()
        {
            //_audioManager.PlayMusic("MainTheme", 1f);
        }

        public async UniTaskVoid OnStartClicked()
        {
            if (!isStartClicked)
            {
                isStartClicked = true;
                await ScreenFader.Instance.FadeInOutProcess(() =>
                {
                    SceneManager.LoadScene("Game");
                });
            }
        }
    }
}
