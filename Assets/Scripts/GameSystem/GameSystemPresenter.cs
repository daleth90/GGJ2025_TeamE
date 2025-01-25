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
            gameSystemModel.GameStartAction += () => gameSystemView.endUI.ShowEndUI(false);
            gameSystemModel.GameStartAction += levelManager.ReStartLevel;
            gameSystemModel.GameStartAction += levelManager.PlayerReStorePosition;
        }

        private void AddGameEndAction()
        {
            gameSystemModel.GameEndAction += () => gameSystemView.endUI.ShowEndUI(true);
        }

        private void AddEndUIButtonEvent()
        {
            gameSystemView.endUI.restartButton.onClick.AddListener(gameSystemModel.GameStartAction);
            gameSystemView.endUI.quitButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
        }

        private void AddGameSystemModelLog()
        {
            gameSystemModel.GameStartAction += gameSystemModel.GameStartLog;
            gameSystemModel.GameEndAction += gameSystemModel.GameEndLog;
        }

        private void Start()
        {
            gameSystemModel.Init();
            gameSystemView.Init();
            levelManager.Init(playerStatus);

            AddGameStartAction();
            AddGameEndAction();
            AddEndUIButtonEvent();
            AddGameSystemModelLog();

            gameSystemModel.GameStartAction();
        }

        [ContextMenu(nameof(TestGameEnd))]
        private void TestGameEnd()
        {
            gameSystemModel.GameEndAction?.Invoke();
        }
    }
}
