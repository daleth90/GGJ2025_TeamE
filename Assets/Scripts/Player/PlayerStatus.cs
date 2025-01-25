using UnityEngine;

namespace Bubble
{
    public class PlayerStatus : MonoBehaviour, IPlayerOxygenStatus
    {
        [SerializeField] private float maxMoveSpeedY = 0;
        [SerializeField] private float maxMoveSpeedX = 0;
        [SerializeField] private float maxSpeed = 0;


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

        public float MaxMoveSpeedX { get => maxMoveSpeedX; set => maxMoveSpeedX = value; }
        public float MaxMoveSpeedY { get => maxMoveSpeedY; set => maxMoveSpeedY = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
    }
}
