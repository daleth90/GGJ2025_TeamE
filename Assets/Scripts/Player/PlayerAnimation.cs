using UnityEngine;

namespace Bubble
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField]
        private PlayerStatus _playerStatus;
        [SerializeField]
        private Animator _characterAnimator;
        [SerializeField]
        private Animator _bubbleAnimator;

        private void Update()
        {
            transform.localScale = _playerStatus.FaceRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

            if (_playerStatus.Object_InertiaX != 0f)
            {
                _characterAnimator.SetBool("Walk", true);
                _bubbleAnimator.SetBool("Walk", true);
            }
            else
            {
                _characterAnimator.SetBool("Walk", false);
                _bubbleAnimator.SetBool("Walk", false);
            }

            //if (_playerStatus.Player_Is_Dash)
            //{
            //    _characterAnimator.SetBool("Dash", true);
            //    _bubbleAnimator.SetBool("Dash", true);
            //}
            //else
            //{
            //    _characterAnimator.SetBool("Dash", false);
            //    _bubbleAnimator.SetBool("Dash", false);
            //}
        }
    }
}
