using UnityEngine;
namespace Bubble
{
    public class Movement_Ctrl : MonoBehaviour
    {
        bool Player_Is_Move = false;
        Vector3 Move_power = new(0, 0, 0), Null_vec3 = new (0, 0, 0);
        void Start()
        {
            Debug.Log("MoveCtrl is Start.");
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                Move_power.z++;
                Player_Is_Move = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                Move_power.z--;
                Player_Is_Move = true;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                Move_power.y++;
                Player_Is_Move = true;
            }
            if (Player_Is_Move) Player_Is_Move = Player_Move();
        }
        private bool Player_Move()
        {
            Debug.Log($" *PlayerMove_Func();\n{Move_power}");
            transform.position += Move_power * Time.deltaTime;
            Move_power = Null_vec3;
            return false;
        }
    }
}
