using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Physalia
{
    public interface IAudioManager
    {
        bool IsFading { get; }
        bool IsMusicMute { get; }
        bool IsSoundMute { get; }
        float MusicVolume { get; }
        float SoundVolume { get; }

        void SetRootTransform(Transform transform);

        UniTask PreloadAudioAsync(string key);
        void UnloadAudio(string key);

        void MuteMusic(bool mute);
        void MuteSound(bool mute);
        void SetMusicVolume(float volume);
        void SetSoundVolume(float volume);

        void PlayMusic(string key, float fadeInTime = 0f);
        void PlayMusic(AudioClip audioClip, float fadeInTime = 0f);
        void StopMusic(float fadeOutTime = 0f);

        void PlaySound(string key, float volumeScale = 1f);
        void PlaySound(AudioClip audioClip, float volumeScale = 1f);
        void PlaySound(string key, Transform transform, float volumeScale = 1f);
        void PlaySound(string key, Vector2 position, float volumeScale = 1f);
        void PlaySound(AudioClip audioClip, Transform transform, float volumeScale = 1f);
        void PlaySound(AudioClip audioClip, Vector2 position, float volumeScale = 1f);

        AudioSource PlaySoundLoop(string key, float volumeScale = 1f);
        AudioSource PlaySoundLoop(string key, Transform transform, float volumeScale = 1f);
        void StopSoundLoop(AudioSource targetSource);
        void StopSoundLoopAll();
    }
}
