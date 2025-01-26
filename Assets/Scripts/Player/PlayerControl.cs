using Physalia;
using UnityEngine;

namespace Bubble
{
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField]
        private PlayerStatus playerStatus;
        [SerializeField]
        private CharacterMovementBody characterMovementBody;
        [SerializeField]
        private bool canMove = true;
        [SerializeField]
        private bool useGravity = true;

        private void Update()
        {
            playerStatus.PlayerDashFrame = false;
            playerStatus.PlayerGroundedFrame = false;

            if (!canMove)
            {
                return;
            }

            if (!playerStatus.IsDashing &&
                Input.GetKeyDown(KeyCode.LeftShift))  // Dash
            {
                playerStatus.DashTime = Time.time;
                playerStatus.PlayerDashFrame = true;
                playerStatus.oxygen -= playerStatus.dashOxygenCost;
            }

            if (playerStatus.IsDashing)
            {
                characterMovementBody.SetLayerMask("Ground");
                UpdateInertiaX_Dash();
            }
            else
            {
                characterMovementBody.SetLayerMask("Ground", "Vine");
                UpdateInertialY();
                UpdateInertialX();
            }

            DoMovement();
        }

        private void UpdateInertiaX_Dash()
        {
            if (playerStatus.FaceRight)
            {
                playerStatus.VelocityX = playerStatus.DashSpeed;
            }
            else
            {
                playerStatus.VelocityX = -playerStatus.DashSpeed;
            }
        }

        private void UpdateInertialY()
        {
            if (Input.GetKey(KeyCode.Space))  // MoveUp
            {
                playerStatus.VelocityY += playerStatus.MoveUpAcceleration * Time.deltaTime;
                playerStatus.oxygen -= playerStatus.upOxygenCost * Time.deltaTime;
                playerStatus.isUp = true;
            }
            else  // Gravity
            {
                if (useGravity)
                {
                    playerStatus.VelocityY -= playerStatus.Gravity * Time.deltaTime;
                }
                playerStatus.isUp = false;
            }
        }

        private void UpdateInertialX()
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))  // Right
            {
                playerStatus.FaceRight = true;
                playerStatus.oxygen -= playerStatus.moveOxygenCost * Time.deltaTime;

                playerStatus.VelocityX += playerStatus.AccelerationX * Time.deltaTime;
                if (playerStatus.VelocityX > playerStatus.MaxMoveSpeedX)
                {
                    playerStatus.VelocityX = playerStatus.MaxMoveSpeedX;
                }
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  // Left
            {
                playerStatus.FaceRight = false;
                playerStatus.oxygen -= playerStatus.moveOxygenCost * Time.deltaTime;

                playerStatus.VelocityX -= playerStatus.AccelerationX * Time.deltaTime;
                if (playerStatus.VelocityX < -playerStatus.MaxMoveSpeedX)
                {
                    playerStatus.VelocityX = -playerStatus.MaxMoveSpeedX;
                }
            }
            else
            {
                // For dash recover
                if (playerStatus.VelocityX > playerStatus.MaxMoveSpeedX)
                {
                    playerStatus.VelocityX = playerStatus.MaxMoveSpeedX;
                }
                else if (playerStatus.VelocityX < -playerStatus.MaxMoveSpeedX)
                {
                    playerStatus.VelocityX = -playerStatus.MaxMoveSpeedX;
                }

                // Normal deceleration
                if (playerStatus.VelocityX > 0)
                {
                    playerStatus.VelocityX -= playerStatus.DecelerationX * Time.deltaTime;
                    if (playerStatus.VelocityX < 0)
                    {
                        playerStatus.VelocityX = 0;
                    }
                }
                else if (playerStatus.VelocityX < 0)
                {
                    playerStatus.VelocityX += playerStatus.DecelerationX * Time.deltaTime;
                    if (playerStatus.VelocityX > 0)
                    {
                        playerStatus.VelocityX = 0;
                    }
                }
            }
        }

        private void DoMovement()
        {
            bool isPreviousGrounded = playerStatus.IsGrounded;

            var moveOffset = new Vector2(playerStatus.VelocityX, playerStatus.VelocityY) * Time.deltaTime;
            var result = characterMovementBody.Move(playerStatus.transform.position, moveOffset.x, moveOffset.y);
            playerStatus.IsGrounded = result.isGrounded;
            playerStatus.PlayerGroundedFrame = !isPreviousGrounded && result.isGrounded;

            if (playerStatus.IsGrounded)
            {
                playerStatus.VelocityY = 0f;
            }

            if (result.isWall)
            {
                // Wall deceleration
                if (playerStatus.VelocityX > 0)
                {
                    playerStatus.VelocityX -= playerStatus.AccelerationX * Time.deltaTime;
                    if (playerStatus.VelocityX < 0)
                    {
                        playerStatus.VelocityX = 0;
                    }
                }
                else if (playerStatus.VelocityX < 0)
                {
                    playerStatus.VelocityX += playerStatus.AccelerationX * Time.deltaTime;
                    if (playerStatus.VelocityX > 0)
                    {
                        playerStatus.VelocityX = 0;
                    }
                }
            }
        }
    }
}
