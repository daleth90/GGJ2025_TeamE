using System;
using UnityEngine;

namespace Bubble
{
    [Serializable]
    public class GameSystemView
    {
        public static GameSystemView instance { get; private set; }

        [field: SerializeField] public EndUI endUI { get; private set; }

        public void Init()
        {
            instance = this;
        }
    }
}
