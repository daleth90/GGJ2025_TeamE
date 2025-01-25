using UnityEngine;
namespace Bubble
{
    public class Movement_Ctrl : MonoBehaviour
    {
        Vector2 MoveDate = new (0, 0), NullVec3 = new(0, 0);

        void Start()
        {
            Debug.Log(" *MoveCtrl is Start.\nPlayer Input (D) => +,\n(A) => -,\n(Space) => MoveUp,\n(W) => Add Bubble,\n(Shift) => Dash,(Shift + D or A) => Big Dash");
        }
        public bool GetInput_Date(float x, float y) { MoveDate += new Vector2(x, y); return true; }
        public bool GetInput_Date(Vector2 Vec2) { MoveDate += Vec2; return true; }
        public bool Player_Move(float MaxSpeed, SpriteRenderer Iamge,bool Is_Dash)
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
                //Vector2 Slow_Power = MoveDate.normalized;
                //float The_Speed_confirm = Mathf.Abs(MoveDate.x) + Mathf.Abs(MoveDate.y), The_Slow_confirm = Mathf.Abs(Slow_Power.x) + Mathf.Abs(Slow_Power.y);
                //while (The_Speed_confirm >= MaxSpeed)
                //{
                //    MoveDate -= Slow_Power;
                //    The_Speed_confirm -= The_Slow_confirm;
                //    Debug.Log("The Speed is to far.");
                //}

            }
            {

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
