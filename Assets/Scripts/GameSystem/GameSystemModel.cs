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
