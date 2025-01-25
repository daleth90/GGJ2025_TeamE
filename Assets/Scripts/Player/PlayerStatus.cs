using UnityEngine;
using UnityEngine.Events;

namespace Bubble
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private Player_Input_Ctrl player_Input_Ctrl;

        [SerializeField] private float maxMoveSpeedY = 10;
        [SerializeField] private float maxMoveSpeedX = 10;

        [SerializeField] private Vector2 object_Inertia = new(0, 0);/*物體慣性移動速度*/
        [SerializeField] private float object_Slow_ForceX = 0;/*減緩移動速度X軸*/
        [SerializeField] private float object_Slow_ForceY = 0;/*減緩移動速度Y軸*/
        [SerializeField] private bool player_Is_Dash = false;
        private int _oxygen = 100;

        private bool isDeath;

        public int oxygen
        {
            get => _oxygen;
            set
            {
                _oxygen = value;
                OnOxygenChanged?.Invoke(_oxygen);
                JudgeDeath();
            }
        }

        public event System.Action<int> OnOxygenChanged;
        public event UnityAction DeathAction;

        public float MaxMoveSpeedX { get => maxMoveSpeedX; set => maxMoveSpeedX = value; }
        public float MaxMoveSpeedY { get => maxMoveSpeedY; set => maxMoveSpeedY = value; }
        public float Object_InertiaX { get => object_Inertia.x; set => object_Inertia.x = value; }
        public float Object_InertiaY { get => object_Inertia.y; set => object_Inertia.y = value; }
        public float Object_Slow_ForceX { get => object_Slow_ForceX; set => object_Slow_ForceX = value; }
        public float Object_Slow_ForceY { get => object_Slow_ForceY; set => object_Slow_ForceY = value; }
        public bool Player_Is_Dash { get => player_Is_Dash; set => player_Is_Dash = value; }
        public  Player_Input_Ctrl Player_Input_Ctrl { get => player_Input_Ctrl; set => player_Input_Ctrl = value; }
        public void Set_Player_Input_Ctrl_Enableed(bool set) { Player_Input_Ctrl.enabled = set; }
        public void Init()
        {
            _oxygen = 100;
            isDeath = true;
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
