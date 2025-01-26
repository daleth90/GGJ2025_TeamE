using System;
using Cysharp.Threading.Tasks;
using Physalia;
using UnityEngine;

namespace Bubble
{
    [Serializable]
    public class GameSystemView
    {
        public static GameSystemView instance { get; private set; }

        [field: SerializeField] public EndUI endUI { get; private set; }

        private IAudioManager audioManager;
        private LevelManager levelManager;

        public void Init(IAudioManager audioManager, LevelManager levelManager)
        {
            instance = this;
            this.audioManager = audioManager;
            this.levelManager = levelManager;
        }

        public void GameStartView()
        {
            audioManager.PlayMusic($"BGM_Level{levelManager.LevelNumber}", 0.2f);
            endUI.Hide();
        }

        public async UniTaskVoid GameSuccessfulViewDelay(int milliseconds)
        {
            await UniTask.Delay(milliseconds);
            GameSuccessfulView();
        }

        public async UniTaskVoid GameFailViewDelay(int milliseconds)
        {
            await UniTask.Delay(milliseconds);
            GameFailView();
        }

        private void GameSuccessfulView()
        {
            audioManager.PlayMusic("BGM_Clear", 0.2f);
            endUI.Show(true);
            endUI.nextButton.gameObject.SetActive(!levelManager.isLastLevel);
        }

        private void GameFailView()
        {
            audioManager.PlayMusic("BGM_GameOver", 0.2f);
            endUI.Show(false);
            endUI.nextButton.gameObject.SetActive(false);
        }
    }
}
