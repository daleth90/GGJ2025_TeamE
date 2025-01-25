using UnityEngine;
namespace Bubble
{
    public class Movement_Ctrl : MonoBehaviour
    {
        Vector3 MoveDate = new (0, 0, 0), NullVec3 = new(0, 0, 0);
        void Start()
        {
            Debug.Log(" *MoveCtrl is Start.\nPlayer Input (D) => +,\n(A) => -,\n(Space) => MoveUp,\n(W) => Add Bubble,\n(Shift) => Dash,(Shift + D or A) => Big Dash");
        }
        public bool GetInput_Date(float x, float y, float z) { MoveDate += new Vector3(x, y, z); return true; }
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
            this.transform.position += MoveDate * Time.deltaTime;/*暫時的*/
            MoveDate = NullVec3;
            return false;
        }
    }
}
