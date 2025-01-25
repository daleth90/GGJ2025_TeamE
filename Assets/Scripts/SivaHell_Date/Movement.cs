using UnityEngine;
namespace Bubble
{
    public class Movement_Ctrl : MonoBehaviour
    {
        Vector3 MoveDate = new (0, 0, 0), NullVec3 = new(0, 0, 0);
        float Maxfloat = 1000;
        void Start()
        {
            Debug.Log(" *MoveCtrl is Start.\nPlayer Input (D) => +,\n(A) => -,\n(Space) => MoveUp,\n(W) => Add Bubble,\n(Shift) => Dash,(Shift + D or A) => Big Dash");
        }
        public bool GetInput_Date(float x, float y, float z) { MoveDate += new Vector3(x, y, z); return true; }
        public bool GetInput_Date(Vector3 Vec3) { MoveDate += Vec3; return true; }
        public bool Player_Move(SpriteRenderer Iamge,bool Is_Dash)
        {
            //Debug.Log($" *PlayerMove_Func();\n{MoveDate},{Is_Dash}");/*Return this.func() is work.*/
            switch (MoveDate.z)/*Update Player Image.*/
            {
                case 1:
                    Iamge.flipX = false;
                    break;
                case -1:
                    Iamge.flipX = true;
                    break;
            }
            if (Is_Dash)/*Add dash power*/
                if (MoveDate.z != 0) MoveDate.z *= 150 * 2;/*Player is on Move*/
                else MoveDate.z += Iamge.flipX ? -150 : 150;/*Player just Input.(Dash)*/
            Move_Speed_confirm();
            Debug.Log(MoveDate * Time.deltaTime);
            this.transform.position += MoveDate * Time.deltaTime;/*暫時的*/
            MoveDate = NullVec3;/*close old.*/
            return false;/*close old.*/
        }
        private void Move_Speed_confirm()
        {
            float The_Speed_confirm = Mathf.Round(MoveDate.x) + Mathf.Round(MoveDate.y) + Mathf.Round(MoveDate.z);
            Vector3 Slow_Power = MoveDate.normalized;
            float The_Slow_confirm = Mathf.Round(Slow_Power.x) + Mathf.Round(Slow_Power.y) + Mathf.Round(Slow_Power.z);
            while (!(Maxfloat >= The_Speed_confirm))
            {
                MoveDate -= Slow_Power;
                The_Speed_confirm -= The_Slow_confirm;
                Debug.Log("The Speed is to far.");
            }
        }
    }
}
