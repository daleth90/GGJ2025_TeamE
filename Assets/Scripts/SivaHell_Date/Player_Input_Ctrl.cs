using UnityEngine;

namespace Bubble
{
    public class Player_Input : MonoBehaviour
    {
        bool Player_Is_Move = false;
        [SerializeField] SpriteRenderer Player_Image;

        [SerializeField] private Movement_Ctrl movement_Ctrl;
        [SerializeField] private PlayerStatus playerStatus;
        [SerializeField] bool Can_Move = true, Hold_Gravity = true;
        private void Start()
        {
            if (Player_Image == null) Player_Image = this.gameObject.GetComponent<SpriteRenderer>();
            movement_Ctrl.Start_Movement_Ctrl(playerStatus);
        }
        private void Update() { Player_Input_Ctrl_(); }
        public void Player_Input_Ctrl_()
        {
            if (Can_Move)
            {
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))/*+z*/
                {
                    Player_Is_Move = movement_Ctrl.GetInput_Date(1,0);
                    Player_Image.flipX = false;
                }
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))/*-z*/
                {
                    Player_Is_Move = movement_Ctrl.GetInput_Date(-1,0);
                    Player_Image.flipX = true;
                }
                if (Input.GetKey(KeyCode.Space))/*MoveUp*/
                {
                    Player_Is_Move = movement_Ctrl.GetInput_Date(0,1);
                }
                if (Input.GetKeyDown(KeyCode.LeftShift)) playerStatus.Player_Is_Dash = true;/*Dash*/
                if (Player_Is_Move || playerStatus.Player_Is_Dash)
                {
                    playerStatus.Player_Is_Dash = Player_Is_Move = movement_Ctrl.Player_Move(Player_Image, playerStatus.Player_Is_Dash);
                    Image_flipX();
                }
                else movement_Ctrl.Player_Move(Hold_Gravity, Player_Image);
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Debug.Log("Add Bubble.");
                }
            }
        }
        private void Image_flipX()
        {
            switch (playerStatus.Object_InertiaX)/*Update Player Image.*/
            {
                case 1:
                    Player_Image.flipX = false;
                    break;
                case -1:
                    Player_Image.flipX = true;
                    break;
            }
        }
    }
}
