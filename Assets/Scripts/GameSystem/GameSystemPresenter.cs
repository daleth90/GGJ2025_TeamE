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
            gameSystemModel.GameSuccessfulAction += gameSystemModel.GameEndAction;
            gameSystemModel.GameSuccessfulAction += gameSystemView.GameSuccessfulView;
        }

        private void AddGameFailAction()
        {
            gameSystemModel.GameFailAction += gameSystemModel.GameEndAction;
            gameSystemModel.GameFailAction += gameSystemView.GameFailView;
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
            gameSystemModel.Init();
            gameSystemView.Init(levelManager);
            levelManager.Init(playerStatus);

            AddGameStartAction();
            AddGameEndAction();
            AddGameFailAction();
            AddGameSuccessfulAction();
            AddEndUIButtonEvent();
            AddPlayerStatusDeathAction();
            AddGameSystemModelLog();

            levelManager.ReStartLevel();
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
