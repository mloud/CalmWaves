using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace Meditation.Apis.Audio
{
    public interface IAudioEnvironmentManager
    {
        AudioMixSettings Settings { get; }
        UniTask Apply(AudioMixSettings settings);
        UniTask Apply(string name);
        UniTask Save(AudioMixSettings settings);
    }
    
    [LogSection("Audio")]
    public class AudioEnvironmentManager : MonoBehaviour, IAudioEnvironmentManager, IService
    {
        public AudioMixSettings Settings { get; private set; }
        
        [SerializeField] AudioMixSettings defaultMix;
        [SerializeField] private List<AudioSource> musicSources;
        [SerializeField] private List<AudioSource> effectSources;

        [SerializeField] private Transform musicSourceContainer;
        [SerializeField] private Transform effectSourceContainer;
        
        // music
        private static Dictionary<string, AddressableAsset<AudioClip>> clipAssets = new();
        private IDataManager dataManager;
        
        public async UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            var mixSettings = await ServiceLocator
                .Get<IDataManager>()
                .GetAll<AudioMixSettings>();
            
            var defaultSettings = mixSettings.FirstOrDefault(x => x.MixName == "default");
            if (defaultSettings == null)
            {
                ServiceLocator.Get<IDataManager>().Add(defaultMix);
            }
            
            effectSources.ForEach(x=>x.enabled = false);
        }
        
        public UniTask PostInitialize() => UniTask.CompletedTask;

      
        public async UniTask Apply(string name)
        {
            var mixSettings = await dataManager.GetAll<AudioMixSettings>();
            var settings = mixSettings.FirstOrDefault(x => x.MixName == name);
            Debug.Assert(settings != null, "No such settings exists", this);
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
 
        private UniTask StopAudio(string effectName, List<AudioSource> audioSources)
        {
            Debug.Log($"XXX stopping audio {effectName}");
            clipAssets.GetValueOrDefault(effectName)?.Release();
            clipAssets.Remove(effectName);

            audioSources.Where(source => source.gameObject.name == effectName)
                .ForEach(source=>
            {
                source.Stop();
                source.clip = null;
                source.enabled = false;
                source.gameObject.SetActive(false);
            });
            return UniTask.CompletedTask;
        }

        private void UpdateAudio(EffectSettings settings, List<AudioSource> audioSources)
        {
            Debug.Log($"XXX updating audio {settings.EffectName}");
            var audioSource = audioSources.Find(x => x.gameObject.name == settings.EffectName);
            Debug.Assert(audioSource != null);
            audioSource.volume = settings.NormalizedVolume;
        }
        
        private async UniTask PlayAudio(EffectSettings settings, List<AudioSource> audioSources, Transform container)
        {
            Debug.Log($"XXX playing audio {settings.EffectName}");
            var audioSource = audioSources.Find(x => x.gameObject.name == settings.EffectName);
            
            if (audioSource == null)
            {
                var sourceGo = new GameObject(settings.EffectName);
                sourceGo.transform.SetParent(container);
                audioSource = sourceGo.AddComponent<AudioSource>();
                audioSources.Add(audioSource);
            }

            audioSource.gameObject.SetActive(true);
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
    }
}