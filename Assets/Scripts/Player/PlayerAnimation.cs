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

        [Space]
        [SerializeField]
        private float _bubbleMinScale = 0.5f;

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
            transform.localScale = _playerStatus.FaceRight ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

            if (_playerStatus.VelocityX != 0f)
            {
                AniRun("Walk", true);
            }
            else
            {
                AniRun("Walk", false);
            }

            if (_playerStatus.PlayerDashFrame)
            {
                AniRun("Dash", true);
                _audioManager.PlaySound("SFX_PlayerDash");
            }
            if (_playerStatus.IsDashing) AniRun("Dash", false);

            if (_playerStatus.PlayerGroundedFrame)
            {
                _audioManager.PlaySound("SFX_PlayerOnGround");
            }
            if (_playerStatus.VelocityX != 0f)
            {
                if (_soundLoopMove == null)
                {
                    _soundLoopMove = _audioManager.PlaySoundLoop("SFX_PlayerMove", transform);
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
            if (_playerStatus.IsDeath) AniRun("Death", true);
            else AniRun("Death", false);
            RefreshBubbleScale();
        }

        private void RefreshBubbleScale()
        {
            float percentage = _playerStatus.oxygen / _playerStatus.MaxOxygen;
            float scale = Mathf.Lerp(_bubbleMinScale, 1f, percentage);
            _bubbleAnimator.transform.localScale = new Vector3(scale, scale, 1f);
        }
        private void AniRun(string Name ,bool B)
        {
            _characterAnimator.SetBool(Name, B);
            _bubbleAnimator.SetBool(Name, B);
        }
    }
}
