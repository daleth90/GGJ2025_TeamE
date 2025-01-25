using UnityEngine;
namespace Bubble
{
    public class Movement_Ctrl : MonoBehaviour
    {
        bool Player_Is_Move = false, Player_Is_Dash = false;
        Vector3 Move_power = new(0, 0, 0), Null_vec3 = new (0, 0, 0);
        [SerializeField] SpriteRenderer Player_Image;

        void Start()
        {
            if (Player_Image == null) Player_Image = this.gameObject.GetComponent<SpriteRenderer>();
            Debug.Log(" *MoveCtrl is Start.\nPlayer Input (D) => +,\n(A) => -,\n(Space) => MoveUp,\n(W) => Add Bubble,\n(Shift) => Dash,(Shift + D or A) => Big Dash");
            Player_Move();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))/*+z*/
            {
                Move_power.z++;
                Player_Is_Move = true;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))/*-z*/
            {
                Move_power.z--;
                Player_Is_Move = true;
            }
            if (Input.GetKey(KeyCode.Space))/*MoveUp*/
            {
                Move_power.y++;
                Player_Is_Move = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) Player_Is_Dash = true;/*Dash*/
            /*Move return to odrer.*/
            if (Player_Is_Move || Player_Is_Dash) Player_Is_Dash = Player_Is_Move = Player_Move();
        }
        private bool Player_Move()
        {
            //Debug.Log($" *PlayerMove_Func();\n{Move_power},{Player_Is_Dash}");/*Return this.func() is work.*/
            switch (Move_power.z)/*Update Player Image.*/
            {
                case 1:
                    Player_Image.flipX = false;
                    break;
                case -1:
                    Player_Image.flipX = true;
                    break;
            }
            if (Player_Is_Dash)/*Add dash power*/
                if (Move_power.z != 0) Move_power.z *= 300;/*Player is on Move*/
                else Move_power.z += Player_Image.flipX ? -150f : 150f;/*Player just Input.(Dash)*/
            transform.position += Move_power * Time.deltaTime;/*暫時的*/
            Move_power = Null_vec3;/*Reset Vec3 date.*/
            return false;
        }
    }
}
