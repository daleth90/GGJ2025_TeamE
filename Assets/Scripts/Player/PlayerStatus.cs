using UnityEngine;
using UnityEngine.Events;

namespace Bubble
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private PlayerControl player_Input_Ctrl;
        [field: SerializeField] public float moveOxygenCost { get; private set; } = 1f;
        [field: SerializeField] public float upOxygenCost { get; private set; } = 1.5f;
        [field: SerializeField] public float dashOxygenCost { get; private set; } = 2f;

        [Space]
        [SerializeField] private int maxOxygen = 100;
        [SerializeField] private float maxMoveSpeedX = 10f;
        [SerializeField] private float maxMoveSpeedY = 5f;
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.2f;

        [Space]
        [SerializeField] private float accelerationX = 4f;
        [SerializeField] private float decelerationX = 2f;
        [SerializeField] private float gravity = 5f;
        [SerializeField] private float moveUpAcceleration = 2f;

        [Space]
        [SerializeField] private bool faceRight = true;
        [SerializeField] private bool isGrounded;
        [SerializeField] private bool playerDashFrame;
        [SerializeField] private bool playerGroundedFrame;

        [Space]
        [SerializeField] private float _oxygen;
        [SerializeField] private Vector2 _velocity;
        [SerializeField] private float _dashTime;

        private bool isDeath;

        public float oxygen
        {
            get => _oxygen;
            set
            {
                _oxygen = Mathf.Clamp(value, 0f, maxOxygen);
                OnOxygenChanged?.Invoke(_oxygen);
                JudgeDeath();
            }
        }

        public event System.Action<float> OnOxygenChanged;
        public event UnityAction DeathAction;

        public int MaxOxygen => maxOxygen;
        public float MaxMoveSpeedX => maxMoveSpeedX;
        public float MaxMoveSpeedY => maxMoveSpeedY;
        public float DashSpeed => dashSpeed;
        public float AccelerationX => accelerationX;
        public float DecelerationX => decelerationX;
        public float Gravity => gravity;
        public float MoveUpAcceleration => moveUpAcceleration;

        public bool FaceRight { get => faceRight; set => faceRight = value; }
        public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
        public bool IsDashing => Time.time - _dashTime < dashDuration;

        public float VelocityX { get => _velocity.x; set => _velocity.x = value; }
        public float VelocityY { get => _velocity.y; set => _velocity.y = value; }
        public float DashTime { get => _dashTime; set => _dashTime = value; }

        public bool PlayerDashFrame { get => playerDashFrame; set => playerDashFrame = value; }
        public bool PlayerGroundedFrame { get => playerGroundedFrame; set => playerGroundedFrame = value; }

        public void Set_Player_Input_Ctrl_Enableed(bool set) { player_Input_Ctrl.enabled = set; }

        public void Init()
        {
            _oxygen = maxOxygen;
            _velocity = Vector2.zero;

            isDeath = false;
            IsGrounded = false;
            PlayerDashFrame = false;
            PlayerGroundedFrame = false;
        }

        private void JudgeDeath()
        {
            if (isDeath) return;
            if (_oxygen > 0) return;

            isDeath = true;

            Set_Player_Input_Ctrl_Enableed(false);
            DeathAction?.Invoke();
        }

        [ContextMenu(nameof(TestDeath))]
        private void TestDeath()
        {
            oxygen = 0;
        }
    }
}
