using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Physalia
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        private struct SoundData
        {
            public Transform transform;
            public float volumeScale;
        }

        private static readonly Logger.Label Label = Logger.Label.CreateFromCurrentClass();

        [SerializeField]
        private AudioSource _musicSource1;
        [SerializeField]
        private AudioSource _musicSource2;
        [SerializeField]
        private AudioSource _soundSource;
        [SerializeField]
        private AudioSource _soundLoopSourcePrefab;

        [Space]
        [SerializeField, ReadOnly]
        private bool _musicMute = false;
        [SerializeField]
        [Range(0f, 1f)]
        private float _musicVolume = 1f;
        [SerializeField, ReadOnly]
        private bool _soundMute = false;
        [SerializeField]
        [Range(0f, 1f)]
        private float _soundVolume = 1f;
        [SerializeField, ReadOnly]
        private AudioSource _currentMusicSource;

        private IAssetManager _assetManager;
        private Transform _rootTransform;
        private ComponentPool<AudioSource> _soundLoopSourcesPool;
        private readonly Dictionary<AudioSource, SoundData> _currentSoundLoopSources = new();
        private readonly Dictionary<string, AudioClip> _audioClipTable = new(32);

        private Coroutine _fadeInCoroutine;
        private Coroutine _fadeOutCoroutine;

        public bool IsFading => _fadeInCoroutine != null || _fadeOutCoroutine != null;
        public bool IsMusicMute => _musicMute;
        public bool IsSoundMute => _soundMute;
        public float MusicVolume => _musicVolume;
        public float SoundVolume => _soundVolume;

        public float MusicTime
        {
            get
            {
                if (_currentMusicSource == null || !_currentMusicSource.isPlaying)
                {
                    return 0f;
                }

                if (_currentMusicSource == _musicSource1)
                {
                    return _musicSource1.time % _musicSource1.clip.length;
                }
                else
                {
                    return _musicSource2.time % _musicSource2.clip.length;
                }
            }
            set
            {
                if (_currentMusicSource == null || !_currentMusicSource.isPlaying)
                {
                    return;
                }

                if (_currentMusicSource == _musicSource1)
                {
                    _musicSource1.time = value;
                }
                else
                {
                    _musicSource2.time = value;
                }
            }
        }

        private void Awake()
        {
            _assetManager = ServiceLocator.Resolve<IAssetManager>();
            _soundLoopSourcesPool = new ComponentPool<AudioSource>(_soundLoopSourcePrefab, 8, transform);
        }

        private void Start()
        {
            if (!_musicSource1.loop)
            {
                Logger.Warn(Label, "_musicSource1.loop != true. Already set it.");
                _musicSource1.loop = true;
            }

            if (!_musicSource2.loop)
            {
                Logger.Warn(Label, "_musicSource2.loop != true. Already set it.");
                _musicSource2.loop = true;
            }

            _currentMusicSource = _musicSource1;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _musicSource1.Stop();
            _musicSource2.Stop();
            _soundSource.Stop();
            StopSoundLoopAll();
        }

        private void OnValidate()
        {
            _musicSource1.volume = _musicVolume;
            _musicSource2.volume = _musicVolume;
        }

        private void LateUpdate()
        {
            if (_rootTransform == null)
            {
                return;
            }

            if (_currentSoundLoopSources.Count == 0)
            {
                return;
            }

            foreach (KeyValuePair<AudioSource, SoundData> pair in _currentSoundLoopSources)
            {
                AudioSource source = pair.Key;
                SoundData data = pair.Value;
                source.volume = data.volumeScale * _soundVolume;
            }
        }

        public void SetRootTransform(Transform transform)
        {
            _rootTransform = transform;
        }

        public async UniTask PreloadAudioAsync(string key)
        {
            if (_audioClipTable.TryGetValue(key, out AudioClip clip))
            {
                if (clip != null)
                {
                    return;
                }
                else
                {
                    // Some body already called this method, but it's still loading.
                    // So we just wait until loading done. (No matter the loading is success or failed)
                    _ = await _assetManager.LoadAssetAsync<AudioClip>(key);
                }
            }
            else
            {
                _audioClipTable.Add(key, null);
                AudioClip loadedClip = await _assetManager.LoadAssetAsync<AudioClip>(key);
                if (loadedClip != null)
                {
                    _audioClipTable[key] = loadedClip;
                }
                else
                {
                    _ = _audioClipTable.Remove(key);
                }
            }
        }

        public void UnloadAudio(string key)
        {
            if (_audioClipTable.TryGetValue(key, out AudioClip clip))
            {
                _ = _audioClipTable.Remove(key);
                if (clip != null)
                {
                    _assetManager.UnloadAsset(key);
                }
            }
        }

        public void MuteMusic(bool mute)
        {
            _musicMute = mute;
            _musicSource1.mute = mute;
            _musicSource2.mute = mute;
        }

        public void MuteSound(bool mute)
        {
            _soundMute = mute;
            _soundSource.mute = mute;
            foreach (var item in _currentSoundLoopSources)
            {
                item.Key.mute = mute;
            }
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
            _currentMusicSource.volume = _musicVolume;
            StartCoroutine(FadeVolume(_currentMusicSource, 1f, volume));
        }

        public void SetSoundVolume(float volume)
        {
            _soundVolume = volume;
        }

        private AudioClip GetAudioClip(string key)
        {
            if (_audioClipTable.TryGetValue(key, out AudioClip clip))
            {
                if (clip != null)
                {
                    return clip;
                }
                else
                {
                    // Some body already called this method, but it's still loading.
                    // So we just wait until loading done. (No matter the loading is success or failed)
                    AudioClip loadedClip = _assetManager.LoadAsset<AudioClip>(key);
                    return loadedClip;
                }
            }
            else
            {
                AudioClip loadedClip = _assetManager.LoadAsset<AudioClip>(key);
                if (loadedClip != null)
                {
                    _audioClipTable.Add(key, loadedClip);
                }

                return loadedClip;
            }
        }

        public void PlayMusic(string key, float fadeInTime = 0f)
        {
            AudioClip audioClip = GetAudioClip(key);
            PlayMusic(audioClip, fadeInTime);
        }

        public void PlayMusic(AudioClip audioClip, float fadeInTime = 0f)
        {
            if (_currentMusicSource.clip == audioClip)
            {
                Logger.Warn(Label, $"Play music canceled! Clip '{audioClip.name}' is already playing.");
                return;
            }

            if (MathUtility.Approximately(fadeInTime, 0f))
            {
                StopFadeIn();
                StopFadeOut();

                _currentMusicSource.clip = audioClip;
                _currentMusicSource.volume = _musicVolume;
                _currentMusicSource.Play();
            }
            else
            {
                PlayMusicWithFadeIn(audioClip, fadeInTime);
            }
        }

        private void PlayMusicWithFadeIn(AudioClip audioClip, float fadeInTime)
        {
            if (_currentMusicSource.isPlaying)
            {
                StopFadeIn();
                StopFadeOut();

                _fadeOutCoroutine = StartCoroutine(FadeOut(_currentMusicSource, fadeInTime));
                _currentMusicSource = _currentMusicSource == _musicSource1 ? _musicSource2 : _musicSource1;
                _fadeInCoroutine = StartCoroutine(FadeIn(_currentMusicSource, audioClip, fadeInTime, _musicVolume));
            }
            else
            {
                StopFadeIn();
                _fadeInCoroutine = StartCoroutine(FadeIn(_currentMusicSource, audioClip, fadeInTime, _musicVolume));
            }
        }

        public void StopMusic(float fadeOutTime = 0f)
        {
            if (MathUtility.Approximately(fadeOutTime, 0f))
            {
                _musicSource1.Stop();
                _musicSource1.clip = null;

                _musicSource2.Stop();
                _musicSource2.clip = null;
            }
            else
            {
                StopMusicWithFadeOut(fadeOutTime);
            }
        }

        private void StopMusicWithFadeOut(float fadeOutTime)
        {
            StopFadeOut();
            _fadeOutCoroutine = StartCoroutine(FadeOut(_currentMusicSource, fadeOutTime));
        }

        private IEnumerator FadeVolume(AudioSource audioSource, float duration, float targetVolume)
        {
            float startVolume = audioSource.volume;

            if (startVolume < targetVolume)
            {
                while (audioSource.volume < targetVolume)
                {
                    audioSource.volume += targetVolume * Time.deltaTime / duration;
                    yield return null;
                }
            }
            else
            {
                while (audioSource.volume > targetVolume)
                {
                    audioSource.volume -= startVolume * Time.deltaTime / duration;
                    yield return null;
                }
            }

            audioSource.volume = targetVolume;
        }

        private void StopFadeIn()
        {
            if (_fadeInCoroutine != null)
            {
                StopCoroutine(_fadeInCoroutine);
                _fadeInCoroutine = null;
            }
        }

        private IEnumerator FadeIn(AudioSource audioSource, AudioClip audioClip, float duration, float targetVolume)
        {
            audioSource.clip = audioClip;
            audioSource.volume = 0f;
            audioSource.Play();

            while (audioSource.volume < targetVolume)
            {
                audioSource.volume += targetVolume * Time.deltaTime / duration;
                yield return null;
            }

            audioSource.volume = targetVolume;

            _fadeInCoroutine = null;
        }

        private void StopFadeOut()
        {
            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
                _fadeOutCoroutine = null;
            }
        }

        private IEnumerator FadeOut(AudioSource audioSource, float duration)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / duration;
                yield return null;
            }

            audioSource.Stop();
            audioSource.clip = null;
            audioSource.volume = startVolume;

            _fadeOutCoroutine = null;
        }

        public void PlaySound(string key, float volumeScale = 1f)
        {
            AudioClip audioClip = GetAudioClip(key);
            PlaySound(audioClip, volumeScale);
        }

        public void PlaySound(string key, Transform transform, float volumeScale = 1f)
        {
            AudioClip audioClip = GetAudioClip(key);
            PlaySound(audioClip, transform, volumeScale);
        }

        public void PlaySound(string key, Vector2 position, float volumeScale = 1f)
        {
            AudioClip audioClip = GetAudioClip(key);
            PlaySound(audioClip, position, volumeScale);
        }

        public void PlaySound(AudioClip audioClip, float volumeScale = 1f)
        {
            if (audioClip != null && !_soundMute)
            {
                _soundSource.PlayOneShot(audioClip, volumeScale * _soundVolume);
            }
        }

        public void PlaySound(AudioClip audioClip, Transform transform, float volumeScale = 1f)
        {
            PlaySound(audioClip, transform.position, volumeScale);
        }

        public void PlaySound(AudioClip audioClip, Vector2 position, float volumeScale = 1f)
        {
            if (audioClip != null && !_soundMute)
            {
                _soundSource.PlayOneShot(audioClip, volumeScale * _soundVolume);
            }
        }

        public AudioSource PlaySoundLoop(string key, float volumeScale = 1f)
        {
            return PlaySoundLoop(key, _rootTransform, volumeScale);
        }

        public AudioSource PlaySoundLoop(string key, Transform transform, float volumeScale = 1f)
        {
            if (transform == null)
            {
                Logger.Warn(Label, "PlaySoundLoop: transform is null. Use rootTransform instead.");
                transform = _rootTransform;
            }

            AudioSource source = _soundLoopSourcesPool.Get();
            source.mute = _soundMute;
            source.gameObject.SetActive(true);
            source.loop = true;

            AudioClip audioClip = GetAudioClip(key);
            source.clip = audioClip;

            var soundData = new SoundData
            {
                transform = transform,
                volumeScale = volumeScale
            };
            _currentSoundLoopSources.Add(source, soundData);

            source.Play();
            return source;
        }

        public void StopSoundLoop(AudioSource targetSource)
        {
            if (_currentSoundLoopSources.ContainsKey(targetSource))
            {
                targetSource.Stop();
                targetSource.clip = null;
                targetSource.gameObject.SetActive(false);
                targetSource.loop = true;

                _currentSoundLoopSources.Remove(targetSource);
                _soundLoopSourcesPool.Release(targetSource);
            }
        }

        public void StopSoundLoopAll()
        {
            foreach (AudioSource source in _currentSoundLoopSources.Keys)
            {
                source.Stop();
                source.clip = null;
                source.gameObject.SetActive(false);
                source.loop = true;
            }

            _currentSoundLoopSources.Clear();
            _soundLoopSourcesPool.ReleaseAll();
        }
    }
}
