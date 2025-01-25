using System;
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
            endUI.ShowEndUI(false);
        }

        public void GameSuccessfulView()
        {
            audioManager.PlayMusic("BGM_Clear", 0.2f);
            endUI.ShowEndUI(true);
            endUI.nextButton.gameObject.SetActive(!levelManager.isLastLevel);
        }

        public void GameFailView()
        {
            audioManager.PlayMusic("BGM_GameOver", 0.2f);
            endUI.ShowEndUI(true);
            endUI.nextButton.gameObject.SetActive(false);
        }
    }
}
