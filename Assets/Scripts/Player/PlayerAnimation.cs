using Physalia;
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

        private IAudioManager _audioManager;
        private AudioSource _soundLoopMove;

        private void Awake()
        {
            _audioManager = ServiceLocator.Resolve<IAudioManager>();
        }

        private void OnDisable()
        {
            if (_soundLoopMove != null)
            {
                _audioManager.StopSoundLoop(_soundLoopMove);
                _soundLoopMove = null;
            }
        }

        private void Update()
        {
            transform.localScale = _playerStatus.FaceRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

            if (_playerStatus.Object_InertiaX != 0f)
            {
                _characterAnimator.SetBool("Walk", true);
                //_bubbleAnimator.SetBool("Walk", true);
            }
            else
            {
                _characterAnimator.SetBool("Walk", false);
                //_bubbleAnimator.SetBool("Walk", false);
            }

            if (_playerStatus.Player_Is_Dash)
            {
                _audioManager.PlaySound("SFX_PlayerDash");
            }

            if (_playerStatus.Object_InertiaX != 0f)
            {
                if (_soundLoopMove == null)
                {
                    _soundLoopMove = _audioManager.PlaySoundLoop("SFX_PlayerMove");
                }
            }
            else
            {
                if (_soundLoopMove != null)
                {
                    _audioManager.StopSoundLoop(_soundLoopMove);
                    _soundLoopMove = null;
                }
            }
        }
    }
}
