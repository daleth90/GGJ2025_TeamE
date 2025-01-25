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

        public void Init()
        {
            instance = this;
        }

        public void GameStartLog()
        {
            Debug.Log("GameStart");
        }

        public void GameEndLog()
        {
            Debug.Log("GameEnd");
        }
    }
}
