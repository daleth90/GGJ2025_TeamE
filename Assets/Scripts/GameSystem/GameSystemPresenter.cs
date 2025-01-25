using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bubble
{

    public class GameSystemPresenter : MonoBehaviour
    {
        [SerializeField] private GameSystemModel gameSystemModel;
        [SerializeField] private GameSystemView gameSystemView;


        private void AddEndUI()
        {
            gameSystemModel.GameStartAction += () => gameSystemView.endUI.ShowEndUI(false);
            gameSystemModel.GameEndAction += () => gameSystemView.endUI.ShowEndUI(true);
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

            AddEndUI();

            AddGameSystemModelLog();

            gameSystemView.endUI.restartButton.onClick.AddListener(gameSystemModel.GameStartAction);
            gameSystemView.endUI.quitButton.onClick.AddListener(() => SceneManager.LoadScene("Menu"));

            gameSystemModel.GameStartAction();
        }

        [ContextMenu(nameof(TestGameEnd))]
        private void TestGameEnd()
        {
            gameSystemModel.GameEndAction?.Invoke();
        }
    }
}
