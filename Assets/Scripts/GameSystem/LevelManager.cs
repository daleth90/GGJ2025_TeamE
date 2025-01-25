using System;
using UnityEngine;

namespace Bubble
{
    [Serializable]
    public class LevelManager
    {
        [SerializeField] private Level[] levelList;

        private Level currentLevel;
        private byte leveNumber = 0;

        public void ReStartLevel()
        {
            LevelLoad(leveNumber);
        }

        public void NextLevel()
        {
            leveNumber += 0;
            LevelLoad(leveNumber);
        }

        private void LevelLoad(byte targetLevelNumber)
        {
            if (leveNumber == targetLevelNumber)
            {
                if (currentLevel != null)
                {
                    UnityEngine.Object.Destroy(currentLevel.gameObject);
                }

                currentLevel = UnityEngine.Object.Instantiate(levelList[targetLevelNumber]);
                leveNumber = targetLevelNumber;
            }
            else
            {
                currentLevel.Inti();
            }
        }
    }
}
