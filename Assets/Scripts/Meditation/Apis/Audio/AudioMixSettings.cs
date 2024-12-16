using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OneDay.Core.Modules.Data;

namespace Meditation.Apis.Audio
{
       
    [Serializable]
    public class EffectSettings
    {
        public string EffectName;
        public float NormalizedVolume;
    }
    
    [Serializable]
    public class AudioMixSettings : BaseDataObject
    {
        [JsonIgnore]
        public Action OnChanged { get; set; }
        public string MixName;

        public List<EffectSettings> Music;
        public List<EffectSettings> Effects;
        
        #region Setters
        public void SetName(string name) => MixName = name;

        public void SetEffectVolume(string audioName, float normalizedVolume) => SetVolume(audioName, normalizedVolume, Effects);
        public void SetMusicVolume(string audioName, float normalizedVolume) => SetVolume(audioName, normalizedVolume, Music);
        
        public void PlayMusic(string audioName, float normalizedVolume) => PlayAudio(audioName, normalizedVolume, Music);
        public void StopMusic(string audioName) => StopAudio(audioName, Music);
        
        public void PlayEffect(string audioName, float normalizedVolume) => PlayAudio(audioName, normalizedVolume, Effects);
        public void StopEffect(string audioName) => StopAudio(audioName, Effects);

        private void SetVolume(string audioName, float normalizedVolume, List<EffectSettings> audioList)
        {
            var audio = audioList.FirstOrDefault(x => x.EffectName == audioName);
            if (audio != null)
            {
                audio.NormalizedVolume = normalizedVolume;
                OnChanged?.Invoke();
            }
        }
        
        private void PlayAudio(string musicName, float normalizedVolume, List<EffectSettings> audioList)
        { 
            var audio = audioList.FirstOrDefault(x => x.EffectName == musicName);
            if (audio != null)
            {
                audio.NormalizedVolume = normalizedVolume;   
            }
            else
            {
                audioList.Add(new EffectSettings
                {
                    EffectName = musicName,
                    NormalizedVolume =  normalizedVolume
                });
            }
            OnChanged?.Invoke();
        }

        private void StopAudio(string musicName, List<EffectSettings> audioList)
        {
            var music = audioList.FirstOrDefault(x => x.EffectName == musicName);
            if (music != null)
            {
                audioList.Remove(music);
            }
            OnChanged?.Invoke();
        }
        #endregion
    }
}