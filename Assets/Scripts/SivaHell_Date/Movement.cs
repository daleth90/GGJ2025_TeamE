using UnityEngine;
namespace Bubble
{
    public class Movement_Ctrl : MonoBehaviour
    {
        Vector2 MoveDate = new (0, 0), NullVec3 = new(0, 0);
        private PlayerStatus playerStatus;

        public void Start_Movement_Ctrl(PlayerStatus playerStatus)
        {
            this.playerStatus = playerStatus;
            Debug.Log(" *MoveCtrl is Start.\nPlayer Input (D) => +,\n(A) => -,\n(Space) => MoveUp,\n(W) => Add Bubble,\n(Shift) => Dash,(Shift + D or A) => Big Dash");
        }
        public bool GetInput_Date(Vector2 Vec2) { MoveDate += Vec2; return true; }
        public bool Player_Move(SpriteRenderer Iamge, bool Is_Dash)
        {
            //Debug.Log($" *PlayerMove_Func();\n{MoveDate},{Is_Dash}");/*Return this.func() is work.*/
            switch (MoveDate.x)/*Update Player Image.*/
            {
                case 1:
                    Iamge.flipX = false;
                    break;
                case -1:
                    Iamge.flipX = true;
                    break;
            }
            if (Is_Dash)/*Add dash power*/
                if (MoveDate.x != 0) MoveDate.x *= 150 * 2;/*Player is on Move*/
                else MoveDate.x += Iamge.flipX ? -150 : 150;/*Player just Input.(Dash)*/
            {
                MoveDate.x = Mathf.Clamp(MoveDate.x, -playerStatus.MaxMoveSpeedX, playerStatus.MaxMoveSpeedX);
                MoveDate.y = Mathf.Clamp(MoveDate.y, -playerStatus.MaxMoveSpeedY, playerStatus.MaxMoveSpeedY);
            }
            Vector3 Vec3 = MoveDate;
            this.transform.position += Vec3 * Time.deltaTime;/*暫時的*/
            MoveDate = NullVec3;/*close old.*/
            return false;/*close old.*/
        }
        //private void Move_Speed_confirm()
        //{
        //    float The_Speed_confirm = Mathf.Round(MoveDate.x) + Mathf.Round(MoveDate.y) + Mathf.Round(MoveDate.z);
        //    Vector3 Slow_Power = MoveDate.normalized;
        //    float The_Slow_confirm = Mathf.Round(Slow_Power.x) + Mathf.Round(Slow_Power.y) + Mathf.Round(Slow_Power.z);
        //    while (!(Maxfloat >= The_Speed_confirm))
        //    {
        //        MoveDate -= Slow_Power;
        //        The_Speed_confirm -= The_Slow_confirm;
        //        Debug.Log("The Speed is to far.");
        //    }
        //}
    }
}
