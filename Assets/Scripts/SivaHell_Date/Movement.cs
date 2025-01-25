using UnityEngine;
namespace Bubble
{
    public class Movement_Ctrl : MonoBehaviour
    {
        [SerializeField]
        private CharacterMovementBody _characterMovementBody;

        Vector2 MoveDate = new(0, 0), NullVec2 = new(0, 0);
        Vector3 Player_Movement = new(0, 0, 0), NullVec3 = new(0, 0, 0);
        private PlayerStatus playerStatus;
        
        public void Start_Movement_Ctrl(PlayerStatus playerStatus)
        {
            this.playerStatus = playerStatus;
            Debug.Log(" *MoveCtrl is Start.\nPlayer Input (D) => +,\n(A) => -,\n(Space) => MoveUp,\n(W) => Add Bubble,\n(Shift) => Dash,(Shift + D or A) => Big Dash");
        }
        public bool GetInput_Date(float x, float y) { MoveDate += new Vector2(x, y); return true; }
        //public bool GetInput_Date(Vector2 Vec2) { MoveDate += Vec2; return true; }
        public bool Player_Move(bool Is_Dash,bool Hold_Gravity)/*Player is Use Button.*/
        {
            //Debug.Log($" *PlayerMove_Func();\n{MoveDate},{Is_Dash}");/*Return this.func() is work.*/
            if (Is_Dash)/*Add dash power*/
            {
                if (MoveDate.x != 0) MoveDate.x *= 2 * 2;/*Player is on Move*/
                else MoveDate.x += playerStatus.FaceRight ? -2 : 2;/*Player just Input.(Dash)*/
                Player_Movement = MoveDate;
            }
            else
            {
                if (MoveDate.x != 0)
                    playerStatus.Object_InertiaX = Mathf.Clamp(playerStatus.Object_InertiaX += MoveDate.x, -playerStatus.MaxMoveSpeedX, playerStatus.MaxMoveSpeedX);
                else Player_Move_SlowX();
                if (MoveDate.y != 0)
                    playerStatus.Object_InertiaY = Mathf.Clamp(playerStatus.Object_InertiaY += MoveDate.y, -playerStatus.MaxMoveSpeedY, playerStatus.MaxMoveSpeedY);
                else Player_Move_SlowY(Hold_Gravity);
                Player_Movement = new Vector2(playerStatus.Object_InertiaX, playerStatus.Object_InertiaY) * Time.deltaTime;
            }

            var result = _characterMovementBody.Move(transform.position, Player_Movement.x, Player_Movement.y);

            MoveDate = NullVec2;/*close old.*/
            Player_Movement = NullVec3;/*close old.*/
            return false;/*close old.*/
        }
        public void Player_Move(bool Hold_Gravity)/*Player not use Any Button.*/
        {
            Player_Move_SlowX();
            Player_Move_SlowY(Hold_Gravity);
            Player_Movement = new Vector2(playerStatus.Object_InertiaX, playerStatus.Object_InertiaY) * Time.deltaTime;

            var result = _characterMovementBody.Move(transform.position, Player_Movement.x, Player_Movement.y);
        }
        private void Player_Move_SlowY(bool Hold_Gravity)
        {
            if (Hold_Gravity)
                _ = playerStatus.Object_InertiaY <= -playerStatus.MaxMoveSpeedY ? playerStatus.Object_InertiaY = -playerStatus.MaxMoveSpeedY : playerStatus.Object_InertiaY -= playerStatus.MaxMoveSpeedY;
            else playerStatus.Object_InertiaY = 0;
        }
        private void Player_Move_SlowX()
        {
            if (playerStatus.Object_InertiaX != 0)
            {
                if (playerStatus.FaceRight) // -x
                {
                    playerStatus.Object_InertiaX += playerStatus.Object_Slow_ForceX;
                    if (playerStatus.Object_InertiaX >= 0)
                    {
                        playerStatus.Object_InertiaX = 0;
                    }
                }
                else  // +x
                {
                    playerStatus.Object_InertiaX -= playerStatus.Object_Slow_ForceX;
                    if (playerStatus.Object_InertiaX <= 0)
                    {
                        playerStatus.Object_InertiaX = 0;
                    }
                }
            }
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
/*
if(A OR D) => +/-x
if(Space) => +
if(Dash) => +/-x

 */
