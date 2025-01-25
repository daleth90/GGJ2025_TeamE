using System;
using UnityEngine;
using UnityEngine.Events;

namespace Bubble
{
    [Serializable]
    public class GameSystemModel
    {
        public static GameSystemModel instance { get; private set; }

        public event UnityAction GameStartAction;
        public event UnityAction GameEndAction;
        public event UnityAction GameSuccessfulAction;
        public event UnityAction GameFailAction;

        public void Init()
        {
            instance = this;
        }

        public void GameStart()
        {
            GameStartAction?.Invoke();
            Debug.Log("Game Start");
        }

        public void GameEnd()
        {
            GameEndAction?.Invoke();
            Debug.Log("Game End");
        }

        public void GameSuccessful()
        {
            GameSuccessfulAction?.Invoke();
            Debug.Log("Game Successful");
        }

        public void GameFail()
        {
            GameFailAction?.Invoke();
            Debug.Log("Game Fail");
        }
    }
}
