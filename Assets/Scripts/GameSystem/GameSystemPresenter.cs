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
            gameSystemModel.GameStartAction += gameSystemView.GameStartView;
            gameSystemModel.GameStartAction += levelManager.PlayerReStorePosition;
        }

        private void AddGameSuccessfulAction()
        {
            gameSystemModel.GameSuccessfulAction += gameSystemView.GameSuccessfulView;
        }

        private void AddGameFailAction()
        {
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

        private void AddGameSystemModelLog()
        {
            gameSystemModel.GameStartAction += gameSystemModel.GameStartLog;
            gameSystemModel.GameFailAction += gameSystemModel.GameSuccessfulLog;
        }

        private void Awake()
        {
            gameSystemModel.Init();
            gameSystemView.Init(levelManager);
            levelManager.Init(playerStatus);

            AddGameStartAction();
            AddGameFailAction();
            AddGameSuccessfulAction();
            AddEndUIButtonEvent();
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
