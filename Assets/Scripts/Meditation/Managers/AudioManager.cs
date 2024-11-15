using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Meditation
{
    public interface IAudioManager
    {
        float SfxVolume { get; set;}
        float MusicVolume { get; set;}
        UniTask PlayMusic(string musicName);
        UniTask PlaySfx(AudioClip clip);
        UniTask StopMusic();
        bool MusicEnabled { get; set; }
        bool SfxEnabled { get; set; }
    }
    public class AudioManager : MonoBehaviour, IService, IAudioManager
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private List<string> musicNames;
        
        [Range(0,1)] [SerializeField] private float musicVolume = 0.5f;
        [Range(0,1)] [SerializeField] private float sfxVolume = 0.5f;

        [SerializeField] private AudioMixer audioMixer;
      
        public float SfxVolume
        {
            get => PlayerPrefs.GetFloat("SfxVolume", 100);
            set
            {
                PlayerPrefs.SetFloat("SfxVolume", value);
                audioMixer.SetFloat("SfxVolume", value);
            }
        }
        
        public float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume", 100);
            set
            {
                PlayerPrefs.SetFloat("MusicVolume", value);
                audioMixer.SetFloat("MusicVolume", value);
            }
        }
        
        private static AddressableAsset<AudioClip> musicAsset = null;
        
        public UniTask Initialize() => UniTask.CompletedTask;
        
        public async UniTask PlayMusic(string musicName)
        {
            if (string.IsNullOrEmpty(musicName))
            {
                musicName = musicNames[Random.Range(0, musicNames.Count)];
            }
            musicAsset?.Release();
            musicAsset = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<AudioClip>(musicName);
            GetAudioSource().clip = musicAsset.GetReference();
            GetAudioSource().Play();
            await DOTween.To(() => GetAudioSource().volume, (t) => GetAudioSource().volume = t, 1, 1.5f)
                .From(0)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }

        public UniTask PlaySfx(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
            return UniTask.CompletedTask;
        }

        public async UniTask StopMusic()
        {
            await DOTween.To(() => GetAudioSource().volume, (t) =>
                {
                    GetAudioSource().volume = t;
                }, 0.0f, 1.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
            GetAudioSource().Stop();
        }

        public static void Release()
        {
            musicAsset?.Release();
        }

        private AudioSource GetAudioSource() => musicSource;

        public bool MusicEnabled
        {
            get => !musicSource.mute;
            set
            {
                if (value)
                    musicSource.mute = false;
                DOTween.To(
                        () => musicSource.volume,
                        t => musicSource.volume = t,
                        value ? 1 : 0, 1.0f)
                    .SetEase(Ease.Linear)
                    .onComplete = () => { musicSource.mute = !value; };
            }
        }
        
        public bool SfxEnabled
        {
            get => !sfxSource.mute;
            set => sfxSource.mute = !value;
        }
    }
}