using System;
using UnityEngine;
using UnityEngine.Events;

namespace Bubble
{
    [Serializable]
    public class GameSystemModel
    {
        public static GameSystemModel instance { get; private set; }

        public UnityAction GameStartAction;
        public UnityAction GameEndAction;
        public UnityAction GameSuccessfulAction;
        public UnityAction GameFailAction;

        public void Init()
        {
            instance = this;
        }

        public void GameStartLog()
        {
            Debug.Log("Game Start");
        }

        public void GameEndLog()
        {
            Debug.Log("Game End");
        }

        public void GameSuccessfulLog()
        {
            Debug.Log("Game Successful");
        }

        public void GameFailLog()
        {
            Debug.Log("Game Fail");
        }
    }
}
