using UnityEngine;

namespace Bubble
{
    public class PlayerStatus : MonoBehaviour, IPlayerMovementData, IPlayerOxygenStatus
    {
        [Header("Movement")]
        [SerializeField] private float _acceleration;
        public float acceleration => _acceleration;

        [SerializeField] private float _maxSpeed;
        public float moveSpeed => _maxSpeed;


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