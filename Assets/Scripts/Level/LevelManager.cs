using System;
using UnityEngine;

namespace Bubble
{
    [Serializable]
    public class LevelManager
    {
        [SerializeField] private Level[] levelList;

        public Level currentLevel { get; private set; }
        public bool isLastLevel { get { return leveNumber == levelList.Length; } private set { } }
        private byte leveNumber = 1;

        private PlayerStatus playerStatus;

        public void Init(PlayerStatus playerStatus)
        {
            this.playerStatus = playerStatus;
        }

        public void ReStartLevel()
        {
            LevelLoad(leveNumber);
        }

        public void NextLevel()
        {
            LevelLoad((byte)(leveNumber + 1));
        }

        public void LevelLoad(byte targetLevelNumber)
        {
            if (leveNumber != targetLevelNumber)
            {
                if (currentLevel != null)
                {
                    UnityEngine.Object.Destroy(currentLevel.gameObject);
                }

                currentLevel = UnityEngine.Object.Instantiate(levelList[targetLevelNumber - 1]);
                leveNumber = targetLevelNumber;
            }
            else
            {
                if(currentLevel != null)
                {
                    currentLevel.Init();
                    return;
                }

                currentLevel = UnityEngine.Object.Instantiate(levelList[targetLevelNumber - 1]);
            }
        }

        public void PlayerReStorePosition()
        {
            playerStatus.transform.position = currentLevel.startPosition.position;
        }
    }
}
