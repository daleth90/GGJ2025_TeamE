using UnityEngine;

namespace Bubble
{
    public class Player_Input : MonoBehaviour
    {
        bool Player_Is_Move = false;
        [SerializeField] SpriteRenderer Player_Image;

        [SerializeField] private Movement_Ctrl movement_Ctrl;
        [SerializeField] private PlayerStatus playerStatus;
        [SerializeField] bool Can_Move = true;
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
                    Player_Is_Move = movement_Ctrl.GetInput_Date(Vector2.right);
                }
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))/*-z*/
                {
                    Player_Is_Move = movement_Ctrl.GetInput_Date(Vector2.left);
                }
                if (Input.GetKey(KeyCode.Space))/*MoveUp*/
                {
                    Player_Is_Move = movement_Ctrl.GetInput_Date(Vector2.up);
                }
                if (Input.GetKeyDown(KeyCode.LeftShift)) playerStatus.Player_Is_Dash = true;/*Dash*/
                if (Player_Is_Move || playerStatus.Player_Is_Dash)
                    playerStatus.Player_Is_Dash = Player_Is_Move = movement_Ctrl.Player_Move(Player_Image, playerStatus.Player_Is_Dash);
                else movement_Ctrl.Player_Move();
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Debug.Log("Add Bubble.");
                }
            }
        }
    }
}
