using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Audio;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Data;
using UnityEngine;
using AudioType = Meditation.Apis.Audio.AudioType;

namespace Meditation.Ui.Audio
{
    public class AudioComponent : MonoBehaviour
    {
     
        [SerializeField] private AudioContentPanel musicContentPanel;
        [SerializeField] private AudioContentPanel effectContentPanel;

        private IAudioEnvironmentManager audioEnvironmentManager;
        private AudioChangedCallbacks musicChangedCallbacks;
        private AudioChangedCallbacks effectsChangedCallbacks;
        
        public async UniTask Initialize(AudioMixSettings mixSettings)
        {
            audioEnvironmentManager = ServiceLocator.Get<IAudioEnvironmentManager>();
            musicChangedCallbacks = new AudioChangedCallbacks
            {
                VolumeChanged = MusicVolumeChanged,
                SelectionChanged = MusicChanged
            };
            
            effectsChangedCallbacks = new AudioChangedCallbacks
            {
                VolumeChanged = EffectVolumeChanged,
                SelectionChanged = EffectChanged
            };
            await PrepareMusic(mixSettings);
            await PrepareEffects(mixSettings);
        }

        private async UniTask PrepareMusic(AudioMixSettings mixSettings)
        {
            var audioDefinitions = await ServiceLocator.Get<IDataManager>()
                .GetAll<AudioDefinition>();
      
            var musicAudio = audioDefinitions
                .Where(x => x.Type == AudioType.Music)
                .ToList();
            
            musicContentPanel.Prepare(musicAudio.Count);
            musicAudio.ForEach((index, definition) =>
            {
                var audioMixSettings = mixSettings.Music.FirstOrDefault(x => x.EffectName == definition.AudioSourceName);
                musicContentPanel.Get(index)
                    .Set(definition, (audioMixSettings != null,audioMixSettings?.NormalizedVolume ?? 0))
                    .AudioCallbacks = musicChangedCallbacks;
            });
        }
        
        private async UniTask PrepareEffects(AudioMixSettings mixSettings)
        {
            var audioDefinitions = await ServiceLocator.Get<IDataManager>()
                .GetAll<AudioDefinition>();
      
            var effectAudio = audioDefinitions
                .Where(x => x.Type == AudioType.Effect)
                .ToList();
            
            effectContentPanel.Prepare(effectAudio.Count);
            effectAudio.ForEach((index, definition) =>
            {
                var audioMixSettings = mixSettings.Effects.FirstOrDefault(x => x.EffectName == definition.AudioSourceName);
                effectContentPanel.Get(index)
                    .Set(definition, (audioMixSettings != null,audioMixSettings?.NormalizedVolume ?? 0))
                    .AudioCallbacks = effectsChangedCallbacks;
            });
        }
        
        private void MusicVolumeChanged(float volume, AudioDefinition definition) => 
            audioEnvironmentManager.Settings.SetMusicVolume(definition.AudioSourceName, volume);
        
        private void EffectVolumeChanged(float volume, AudioDefinition definition) => 
            audioEnvironmentManager.Settings.SetEffectVolume(definition.AudioSourceName, volume);

        private void MusicChanged(bool isOn, AudioDefinition definition)
        {
            if (isOn)
                audioEnvironmentManager.Settings.PlayMusic(definition.AudioSourceName, 0.5f);
            else
                audioEnvironmentManager.Settings.StopMusic(definition.AudioSourceName);
        }
        
        private void EffectChanged(bool isOn, AudioDefinition definition)
        {
            if (isOn)
                audioEnvironmentManager.Settings.PlayEffect(definition.AudioSourceName, 0.5f);
            else
                audioEnvironmentManager.Settings.StopEffect(definition.AudioSourceName);
        }
    }
}