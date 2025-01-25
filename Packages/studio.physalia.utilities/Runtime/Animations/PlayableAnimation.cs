using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Physalia
{
    [RequireComponent(typeof(Animator))]
    public class PlayableAnimation : MonoBehaviour, IAnimationClipSource
    {
        private const string OutputName = "SimpleAnimation";

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private AnimationClip _current;
        [SerializeField]
        private AnimationClip[] _animationClips;
        [SerializeField]
        private bool _playAutomatically = true;

        private bool _isGraphInitialized;
        private PlayableGraph _playableGraph;

        public bool IsPlaying => _playableGraph.IsPlaying();
        public bool IsDone => _playableGraph.IsDone();

        private void Awake()
        {
            if (!_isGraphInitialized)
            {
                InitializeGraph();
            }
        }

        private void OnDestroy()
        {
            _playableGraph.Destroy();
        }

        private void OnValidate()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            if (_playableGraph.IsValid())
            {
                if (_current != GetPlayingClip())
                {
                    SetCurrent(_current);
                }
            }
        }

        private void InitializeGraph()
        {
            _isGraphInitialized = true;
            _playableGraph = PlayableGraph.Create();
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            _ = AnimationPlayableOutput.Create(_playableGraph, OutputName, _animator);
        }

        private AnimationClip GetPlayingClip()
        {
            PlayableOutput output = _playableGraph.GetOutput(0);
            if (output.Equals(PlayableOutput.Null))
            {
                return null;
            }

            AnimationClipPlayable playable = (AnimationClipPlayable)output.GetSourcePlayable();
            return playable.GetAnimationClip();
        }

        public void GetAnimationClips(List<AnimationClip> results)
        {
            results.Clear();
            results.AddRange(_animationClips);
            if (_current != null && !results.Contains(_current))
            {
                results.Add(_current);
            }
        }

        private void OnEnable()
        {
            SetupPlayableWithCurrent();
            if (_playAutomatically)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        private void LateUpdate()
        {
            if (_playableGraph.IsValid())
            {
                if (_playableGraph.IsPlaying() && _playableGraph.IsDone())
                {
                    _playableGraph.Stop();
                }
            }
        }

        public void Play()
        {
            if (!_isGraphInitialized)
            {
                InitializeGraph();
            }

            _playableGraph.Play();
        }

        public void Stop()
        {
            if (!_isGraphInitialized)
            {
                return;
            }

            SetNormalizedTime(0f);
            _playableGraph.Evaluate(0f);
            _playableGraph.Stop();
        }

        public void Pause()
        {
            if (!_isGraphInitialized)
            {
                return;
            }

            _playableGraph.Stop();
        }

        public void PlayFromZero()
        {
            if (!_isGraphInitialized)
            {
                InitializeGraph();
            }

            SetNormalizedTime(0f);
            _playableGraph.Evaluate(0f);
            _playableGraph.Play();
        }

        public AnimationClip GetCurrent()
        {
            return _current;
        }

        public void SetCurrent(int index)
        {
            if (index < 0 || index >= _animationClips.Length)
            {
                SetCurrent(null);
                return;
            }

            AnimationClip clip = _animationClips[index];
            SetCurrent(clip);
        }

        public void SetCurrent(AnimationClip animationClip)
        {
            if (_current == animationClip)
            {
                return;
            }

            if (!_isGraphInitialized)
            {
                InitializeGraph();
            }

            _current = animationClip;
            SetupPlayableWithCurrent();
            if (_playAutomatically)
            {
                Play();
            }
        }

        public void SetSpeed(float speed)
        {
            if (!_isGraphInitialized)
            {
                InitializeGraph();
            }

            int index = _playableGraph.GetRootPlayableCount() - 1;
            if (index >= 0)
            {
                Playable playable = _playableGraph.GetRootPlayable(index);
                if (!playable.Equals(Playable.Null))
                {
                    playable.SetSpeed(speed);
                }
            }
        }

        public float GetNormalizedTime()
        {
            if (_current == null)
            {
                return 0f;
            }

            if (!_isGraphInitialized)
            {
                InitializeGraph();
            }

            int index = _playableGraph.GetRootPlayableCount() - 1;
            if (index < 0)
            {
                return 0f;
            }

            Playable playable = _playableGraph.GetRootPlayable(index);
            if (playable.Equals(Playable.Null))
            {
                return 0f;
            }

            float time = (float)playable.GetTime();
            return time / _current.length;
        }

        public void SetNormalizedTime(float normalizedTime)
        {
            if (_current == null)
            {
                return;
            }

            if (!_isGraphInitialized)
            {
                InitializeGraph();
            }

            normalizedTime = Mathf.Clamp01(normalizedTime);
            float time = _current.length * normalizedTime;

            int index = _playableGraph.GetRootPlayableCount() - 1;
            if (index >= 0)
            {
                Playable playable = _playableGraph.GetRootPlayable(index);
                if (!playable.Equals(Playable.Null))
                {
                    playable.SetTime(time);
                }
            }
        }

        private void SetupPlayableWithCurrent()
        {
            PlayableOutput output = _playableGraph.GetOutput(0);
            if (_current == null)
            {
                output.SetSourcePlayable(Playable.Null);
                return;
            }

            AnimationClipPlayable playable = AnimationClipPlayable.Create(_playableGraph, _current);
            if (!_current.isLooping)
            {
                // If we do not set duration, IsDone() will never return true.
                playable.SetDuration(_current.length);
            }

            output.SetSourcePlayable(playable);
        }

        public void Evaluate(float deltaTime = 0f)
        {
            if (!_isGraphInitialized)
            {
                InitializeGraph();
            }

            _playableGraph.Evaluate(deltaTime);
        }
    }
}
