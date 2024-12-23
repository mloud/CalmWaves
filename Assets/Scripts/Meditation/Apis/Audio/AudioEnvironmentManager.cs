using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Data;
using UnityEngine;
using UnityEngine.Audio;

namespace Meditation.Apis.Audio
{
    public interface IAudioEnvironmentManager
    {
        float NormalizedVolume { get; set; }
        AudioMixSettings Settings { get; }
        UniTask Apply(AudioMixSettings settings);
        UniTask Apply(string name);
        UniTask Save(AudioMixSettings settings);

        UniTask SetMuted(bool isMuted, float duration);
        UniTask StopAll(float duration);
    }
    
    [LogSection("Audio")]
    public class AudioEnvironmentManager : MonoBehaviour, IAudioEnvironmentManager, IService
    {
        public float NormalizedVolume
        {
            get => GetVolume();
            set => SetVolume(value);
        }
        public AudioMixSettings Settings { get; private set; }

        [SerializeField] private AudioMixerGroup audioMixerGroup;
        [SerializeField] private List<AudioMixSettings> defaultMixes;
        [SerializeField] private List<AudioSource> musicSources;
        [SerializeField] private List<AudioSource> effectSources;

        [SerializeField] private Transform musicSourceContainer;
        [SerializeField] private Transform effectSourceContainer;
        
        // music
        private static Dictionary<string, AddressableAsset<AudioClip>> clipAssets = new();
        private IDataManager dataManager;
        private bool willUnmuteAfterResume;
        private float volume;
        
        public async UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            var mixSettings = await ServiceLocator
                .Get<IDataManager>()
                .GetAll<AudioMixSettings>();
            
            var defaultSettings = mixSettings.FirstOrDefault(x => x.MixName == "default");
            if (defaultSettings == null)
            {
                ServiceLocator.Get<IDataManager>().Add(defaultMixes.First(x=>x.MixName == "default"));
            }
            
            var sleepDefaultSettings = mixSettings.FirstOrDefault(x => x.MixName == "sleepDefault");
            if (sleepDefaultSettings == null)
            {
                ServiceLocator.Get<IDataManager>().Add(defaultMixes.First(x=>x.MixName == "sleepDefault"));
            }
            effectSources.ForEach(x=>x.enabled = false);
        }
        
        public UniTask PostInitialize() => UniTask.CompletedTask;

      
        public async UniTask Apply(string name)
        {
            var mixSettings = await dataManager.GetAll<AudioMixSettings>();
            var settings = mixSettings.FirstOrDefault(x => x.MixName == name);

            if (settings == null)
            {
                settings = defaultMixes.FirstOrDefault(x => x.MixName == name);
            }
            Debug.Assert(settings != null, "No such settings exists", this);
            D.LogInfo($"Applying mix {JsonConvert.SerializeObject(settings)}", this);
            await Apply(settings);
        }

        public async UniTask Save(AudioMixSettings settings)
        {
            var mixSettings = await ServiceLocator.Get<IDataManager>().GetAll<AudioMixSettings>();
            if (mixSettings.FirstOrDefault(x => x.MixName == settings.MixName) == null)
            {
                dataManager.Add(settings);
            }
            else
            {
                dataManager.Actualize(settings);
            }
        }

        public async UniTask SetMuted(bool isMuted, float duration)
        {
            float finalVolume = isMuted ? -80 : 0;
            await DOTween.To(GetVolume, SetVolume, finalVolume, duration).AsyncWaitForCompletion();
        }
      
        public async UniTask StopAll(float duration)
        {
            await UniTask.WhenAll(
                StopAudio("*", musicSources, duration),
                StopAudio("*", effectSources, duration));
        }

        public UniTask SaveCurrent(AudioMixSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask Apply(AudioMixSettings settings)
        {
            Settings = settings;
            Settings.OnChanged = () => Apply(Settings);
           
            // effects
            var activeEffectsNow = effectSources.Where(x => x.clip != null).Select(x => x.gameObject.name)
                .ToList();

            var activeEffectsNew = settings.Effects.Select(x => x.EffectName)
                .ToList();
            
            var effectToStop = activeEffectsNow.Where(x => !activeEffectsNew.Contains(x));
            effectToStop.ForEach(effectName=>StopAudio(effectName, effectSources));
          
            var effectsToStart = activeEffectsNew.Where(x => !activeEffectsNow.Contains(x));
            effectsToStart.ForEach(effectName=>PlayAudio(settings.Effects.First(x=>x.EffectName == effectName),effectSources, effectSourceContainer));
            
            // update
            var effectsToUpdate = activeEffectsNew.Where(x => activeEffectsNow.Contains(x));
            effectsToUpdate.ForEach(effectName=>UpdateAudio(settings.Effects.First(x=>x.EffectName == effectName), effectSources));
       
            // music
            var activeMusicNow = musicSources.Where(x => x.clip != null).Select(x => x.gameObject.name)
                .ToList();

            var activeMusicNew = settings.Music.Select(x => x.EffectName)
                .ToList();
            
            // stop music that is not active anymore
            var musicToStop = activeMusicNow.Where(x => !activeMusicNew.Contains(x));
            musicToStop.ForEach(effectName=>StopAudio(effectName, musicSources));
          
            // start new music
            var musicToStart = activeMusicNew.Where(x => !activeMusicNow.Contains(x));
            musicToStart.ForEach(effectName=>PlayAudio(settings.Music.First(x=>x.EffectName == effectName),musicSources, musicSourceContainer));
            
            // update
            var musicToUpdate = activeMusicNew.Where(x => activeMusicNow.Contains(x));
            musicToUpdate.ForEach(effectName=>UpdateAudio(settings.Music.First(x=>x.EffectName == effectName), musicSources));
        }
 
        private async UniTask StopAudio(string effectName, List<AudioSource> audioSources, float duration = 1.0f)
        {
            var tasks = audioSources.Where(source => effectName == "*" || source.gameObject.name == effectName)
                .Select(source=>DOTween.To(() => source.volume, t => source.volume = t, 0, 1f)
                .SetEase(Ease.Linear)
                .ToUniTask());

            await UniTask.WhenAll(tasks);
           
            audioSources.Where(source => source.gameObject.name == effectName)
                .ForEach(source=>
            {
                source.clip = null;
                source.enabled = false;
                source.gameObject.SetActive(false);
            });
            
            clipAssets.GetValueOrDefault(effectName)?.Release();
            clipAssets.Remove(effectName);
    }

        private void UpdateAudio(EffectSettings settings, List<AudioSource> audioSources)
        {
            var audioSource = audioSources.Find(x => x.gameObject.name == settings.EffectName);
            Debug.Assert(audioSource != null);
            audioSource.volume = settings.NormalizedVolume;
        }
        
        private async UniTask PlayAudio(EffectSettings settings, List<AudioSource> audioSources, Transform container)
        {
            var audioSource = audioSources.Find(x => x.gameObject.name == settings.EffectName);
            
            if (audioSource == null)
            {
                var sourceGo = new GameObject(settings.EffectName);
                sourceGo.transform.SetParent(container);
                audioSource = sourceGo.AddComponent<AudioSource>();
                audioSources.Add(audioSource);
            }

            audioSource.gameObject.SetActive(true);
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.enabled = true;
            audioSource.volume = settings.NormalizedVolume;
            audioSource.loop = true;

            if (audioSource.clip == null)
            {
                var clipAsset = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<AudioClip>(settings.EffectName);
                if (!clipAssets.ContainsKey(settings.EffectName))
                {
                    clipAssets.Add(clipAsset.GetReference().name, clipAsset);
                }
                else
                {
                    clipAsset.Release();
                    clipAsset = clipAssets[settings.EffectName];
                }
                audioSource.clip = clipAsset.GetReference();
            }
           
            audioSource.Play();
       
            await DOTween.To(() => audioSource.volume, (t) => audioSource.volume = t, settings.NormalizedVolume, 1f)
                .From(0)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                willUnmuteAfterResume = GetVolume() > 0.001f;
                if (willUnmuteAfterResume)
                {
                    volume = GetVolume();
                    SetVolume(0);
                }
            }
            else
            {
                if (willUnmuteAfterResume)
                {
                    DOTween.To(GetVolume, SetVolume, volume, 3.0f).From(0);
                }
            }
        }

       
        private float GetVolume()
        {
            audioMixerGroup.audioMixer.GetFloat("EnvironmentVolume", out float currentVolume);
            return Mathf.Pow(10, currentVolume / 20);
        }
            
        private void SetVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0.0001f, 1.0f); // Ensure targetLinear is > 0
            audioMixerGroup.audioMixer.SetFloat("EnvironmentVolume", 20f * Mathf.Log10(volume));
        }
    }
}