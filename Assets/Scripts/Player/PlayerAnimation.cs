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

        private UpFX upFX = new();
        private DashFX dashFX = new();

        private void Awake()
        {
            _audioManager = ServiceLocator.Resolve<IAudioManager>();
            upFX.Init(transform);
            dashFX.Init(transform);
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
            if (_playerStatus.IsDeath)
            {
                if (!_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    _characterAnimator.Play("Death", 0);
                }
            }
            else
            {
                if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    _characterAnimator.Play("Idle", 0);
                }
            }

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
                dashFX.Play();
                _audioManager.PlaySound("SFX_PlayerDash");
            }

            if (_playerStatus.IsDashing) AniRun("Dash", true);
            else AniRun("Dash", false);

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

            if (_playerStatus.isUp)
            {
                upFX.Play();
            }
            else
            {
                upFX.Stop();
            }

            RefreshBubbleScale();
        }

        private void OnDestroy()
        {
            upFX.OnDestory();
            dashFX.OnDestory();
        }

        private void RefreshBubbleScale()
        {
            float percentage = _playerStatus.oxygen / _playerStatus.MaxOxygen;
            float scale = Mathf.Lerp(_bubbleMinScale, 1f, percentage);
            _bubbleAnimator.transform.localScale = new Vector3(scale, scale, 1f);
        }
        private void AniRun(string Name, bool B)
        {
            _characterAnimator.SetBool(Name, B);
            _bubbleAnimator.SetBool(Name, B);
        }
    }
}
