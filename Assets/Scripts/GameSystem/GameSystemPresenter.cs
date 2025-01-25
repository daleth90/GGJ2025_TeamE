using Physalia;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bubble
{

    public class GameSystemPresenter : MonoBehaviour
    {
        [SerializeField] private GameSystemModel gameSystemModel;
        [SerializeField] private GameSystemView gameSystemView;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private PlayerStatus playerStatus;

        private IAudioManager _audioManager;

        private void AddGameStartAction()
        {
            gameSystemModel.GameStartAction += levelManager.PlayerReStorePosition;
            gameSystemModel.GameStartAction += playerStatus.Init;
            gameSystemModel.GameStartAction += () => playerStatus.gameObject.SetActive(true);
            gameSystemModel.GameStartAction += () => playerStatus.Set_Player_Input_Ctrl_Enableed(true);
            gameSystemModel.GameStartAction += gameSystemView.GameStartView;
        }

        private void AddGameEndAction()
        {
            gameSystemModel.GameEndAction += () => playerStatus.gameObject.SetActive(false);
            gameSystemModel.GameEndAction += () => playerStatus.Set_Player_Input_Ctrl_Enableed(false);
        }

        private void AddGameSuccessfulAction()
        {
            gameSystemModel.GameSuccessfulAction += () => _audioManager.PlaySound("SFX_Goal");
            gameSystemModel.GameSuccessfulAction += gameSystemModel.GameEndAction;
            gameSystemModel.GameSuccessfulAction += () => gameSystemView.GameSuccessfulViewDelay(500).Forget();
        }

        private void AddGameFailAction()
        {
            gameSystemModel.GameFailAction += () => _audioManager.PlaySound("SFX_PlayerDeath");
            gameSystemModel.GameFailAction += gameSystemModel.GameEndAction;
            gameSystemModel.GameFailAction += () => gameSystemView.GameFailViewDelay(500).Forget();
        }

        private void AddEndUIButtonEvent()
        {
            gameSystemView.endUI.restartButton.onClick.AddListener(levelManager.ReStartLevel);
            gameSystemView.endUI.restartButton.onClick.AddListener(gameSystemModel.GameStartAction);
            
            gameSystemView.endUI.nextButton.onClick.AddListener(levelManager.NextLevel);
            gameSystemView.endUI.nextButton.onClick.AddListener(gameSystemModel.GameStartAction);

            gameSystemView.endUI.quitButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
        }

        private void AddPlayerStatusDeathAction()
        {
            playerStatus.DeathAction += gameSystemModel.GameFailAction;
        }

        private void AddGameSystemModelLog()
        {
            gameSystemModel.GameStartAction += gameSystemModel.GameStartLog;
            gameSystemModel.GameEndAction += gameSystemModel.GameEndLog;
            gameSystemModel.GameSuccessfulAction += gameSystemModel.GameSuccessfulLog;
            gameSystemModel.GameFailAction += gameSystemModel.GameFailLog;
        }

        private void Awake()
        {
            _audioManager = ServiceLocator.Resolve<IAudioManager>();
            gameSystemModel.Init();
            gameSystemView.Init(_audioManager, levelManager);
            levelManager.Init(playerStatus);

            levelManager.ReStartLevel();

            AddGameStartAction();
            AddGameEndAction();
            AddGameFailAction();
            AddGameSuccessfulAction();
            AddEndUIButtonEvent();
            AddPlayerStatusDeathAction();
            AddGameSystemModelLog();
        }

        private void Start()
        {
            gameSystemModel.GameStartAction();
        }

        [ContextMenu(nameof(GameSystemModel.GameSuccessfulAction))]
        private void TestGameSuccessfulAction()
        {
            gameSystemModel.GameSuccessfulAction?.Invoke();
        }

        [ContextMenu(nameof(GameSystemModel.GameFailAction))]
        private void TestGameFailAction()
        {
            gameSystemModel.GameFailAction?.Invoke();
        }
    }
}
