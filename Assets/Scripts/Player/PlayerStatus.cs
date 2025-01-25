using UnityEngine;

namespace Bubble
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private float maxMoveSpeedY = 10;
        [SerializeField] private float maxMoveSpeedX = 10;
        [SerializeField] private float maxSpeed = 0;
        public float MaxMoveSpeedX { get => maxMoveSpeedX; set => maxMoveSpeedX = value; }
        public float MaxMoveSpeedY { get => maxMoveSpeedY; set => maxMoveSpeedY = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        public bool isInDash;

        private int _oxygen;
        public int oxygen
        {
            get => _oxygen;
            set
            {
                _oxygen = value;
                OnOxygenChanged.Invoke(_oxygen);
            }
        }

        public event System.Action<int> OnOxygenChanged;
    }
}
