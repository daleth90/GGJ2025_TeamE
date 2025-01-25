using UnityEngine;

namespace Bubble
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField]
        private Animator _characterAnimator;
        [SerializeField]
        private Animator _bubbleAnimator;

        public void PlayIdle()
        {
            _characterAnimator.Play("Move");
        }

        public void PlayMove()
        {
            _characterAnimator.Play("Move");
        }

        public void PlayFloat()
        {
            _bubbleAnimator.Play("Float");
        }
    }
}
