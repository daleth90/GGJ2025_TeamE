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
        public bool Player_Move(SpriteRenderer Iamge, bool Is_Dash)/*Player is Use Button.*/
        {
            //Debug.Log($" *PlayerMove_Func();\n{MoveDate},{Is_Dash}");/*Return this.func() is work.*/
            if (Is_Dash)/*Add dash power*/
            {
                if (MoveDate.x != 0) MoveDate.x *= 2 * 2;/*Player is on Move*/
                else MoveDate.x += Iamge.flipX ? -2 : 2;/*Player just Input.(Dash)*/
                Player_Movement = MoveDate;
            }
            else
            {
                playerStatus.Object_InertiaX += MoveDate.x;
                playerStatus.Object_InertiaY += MoveDate.y;
                playerStatus.Object_InertiaX = Mathf.Clamp(playerStatus.Object_InertiaX, -playerStatus.MaxMoveSpeedX, playerStatus.MaxMoveSpeedX);
                playerStatus.Object_InertiaY = Mathf.Clamp(playerStatus.Object_InertiaY, -playerStatus.MaxMoveSpeedY, playerStatus.MaxMoveSpeedY);
                Player_Movement = new Vector2(playerStatus.Object_InertiaX, playerStatus.Object_InertiaY) * Time.deltaTime;
            }

            var result = _characterMovementBody.Move(transform.position, Player_Movement.x, Player_Movement.y);

            MoveDate = NullVec2;/*close old.*/
            Player_Movement = NullVec3;/*close old.*/
            return false;/*close old.*/
        }
        public void Player_Move(bool Hold_Gravity, SpriteRenderer Iamge)/*Player not use Any Button.*/
        {
            if (playerStatus.Object_InertiaX != 0)
            {
                if (Iamge.flipX) // -x
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
            if (Hold_Gravity)
                _ = playerStatus.Object_InertiaY <= -playerStatus.MaxMoveSpeedY ? playerStatus.Object_InertiaY = -playerStatus.MaxMoveSpeedY : playerStatus.Object_InertiaY -= playerStatus.MaxMoveSpeedY;
            else playerStatus.Object_InertiaY = 0;
            Player_Movement = new Vector2(playerStatus.Object_InertiaX, playerStatus.Object_InertiaY) * Time.deltaTime;

            var result = _characterMovementBody.Move(transform.position, Player_Movement.x, Player_Movement.y);
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
