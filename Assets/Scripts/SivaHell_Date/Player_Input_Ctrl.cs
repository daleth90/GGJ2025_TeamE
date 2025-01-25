using UnityEngine;

namespace Bubble
{
    public class Player_Input_Ctrl : MonoBehaviour
    {
        bool Player_Is_Move = false, Player_Is_Dash = false;
        [SerializeField] SpriteRenderer Player_Image;

        [SerializeField] private Movement_Ctrl movement_Ctrl;
        [SerializeField] bool Can_Move = true;
        private void Start()
        {
           // movement_Ctrl = new Movement_Ctrl();
            if (Player_Image == null) Player_Image = this.gameObject.GetComponent<SpriteRenderer>();
        }
        private void Update()
        {
            Player_Input_Ctrl_();
        }
        public void Player_Input_Ctrl_()
        {
            if (Can_Move)
            {
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))/*+z*/
                {
                    Player_Is_Move = movement_Ctrl.GetInput_Date(0, 0, 1);
                }
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))/*-z*/
                {
                    Player_Is_Move = movement_Ctrl.GetInput_Date(0, 0, -1);
                }
                if (Input.GetKey(KeyCode.Space))/*MoveUp*/
                {
                    Player_Is_Move = movement_Ctrl.GetInput_Date(0, 1, 0);
                }
                if (Input.GetKeyDown(KeyCode.LeftShift)) Player_Is_Dash = true;/*Dash*/
                if (Player_Is_Move || Player_Is_Dash) Player_Is_Dash = Player_Is_Move = movement_Ctrl.Player_Move(Player_Image, Player_Is_Dash);
            }
        }
    }
    enum Player_
    {
        OnRun, Jump
    }
}
