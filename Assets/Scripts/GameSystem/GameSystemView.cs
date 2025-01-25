using System;
using UnityEngine;

namespace Bubble
{
    [Serializable]
    public class GameSystemView
    {
        public static GameSystemView instance { get; private set; }

        [field: SerializeField] public EndUI endUI { get; private set; }

        private LevelManager levelManager;

        public void Init(LevelManager levelManager)
        {
            instance = this;
            this.levelManager = levelManager;
        }

        public void GameStartView()
        {
            endUI.ShowEndUI(false);
        }

        public void GameSuccessfulView()
        {
            endUI.ShowEndUI(true);
            endUI.nextButton.gameObject.SetActive(!levelManager.isLastLevel);
        }

        public void GameFailView()
        {
            endUI.ShowEndUI(true);
            endUI.nextButton.gameObject.SetActive(false);
        }
    }
}
