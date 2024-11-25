using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using UnityEngine;

namespace Meditation.Apis.Settings
{
    public enum Mode
    {
        Voice_Male,
        Voice_Female,
        Beep
    }
    
    public interface ISettingsModule
    { }

    public interface IVolumeModule : ISettingsModule
    {
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }

    }

    public class VolumeModule: IVolumeModule
    {
        public float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume", 100);
            set => PlayerPrefs.SetFloat("MusicVolume", value);
        }
        
        public float SfxVolume
        {
            get => PlayerPrefs.GetFloat("SfxVolume", 100);
            set => PlayerPrefs.SetFloat("SfxVolume", value);
        }
    }
    
    public interface ISoundSettingsModule : ISettingsModule
    {
        string GetBreathingFinishClip();
        string GetInhaleClip();
        string GetExhaleClip();
        string GetHoldClip();
        Mode BreathingSoundMode { get; set; }
    }

    public class SettingsModule : ISoundSettingsModule
    {
        public Mode BreathingSoundMode 
        { 
            get => (Mode)PlayerPrefs.GetInt("SoundStyle", 0);
            set => PlayerPrefs.SetInt("SoundStyle", (int)value);
        }

        public string GetBreathingFinishClip() => "win";
  
        public string GetInhaleClip() =>
            BreathingSoundMode switch
            {
                Mode.Beep => "beep",
                Mode.Voice_Male => "inhale_male",
                Mode.Voice_Female => "inhale_female",
                _ => null
            };

        public string GetExhaleClip() =>
            BreathingSoundMode switch
            {
                Mode.Beep => "beep",
                Mode.Voice_Male => "exhale_male",
                Mode.Voice_Female => "exhale_female",
                _ => null
            };

        public string GetHoldClip() =>
            BreathingSoundMode switch
            {
                Mode.Beep => "beep",
                Mode.Voice_Male => "hold_male",
                Mode.Voice_Female => "hold_female",
                _ => null
            };
    }
    
    public interface ISettingsApi
    {
        T GetModule<T>() where T : ISettingsModule;
        void RegisterModule<T>(ISettingsModule module) where T: ISettingsModule;
    }

    public class SettingsApi : MonoBehaviour, ISettingsApi, IService
    {
        private Dictionary<Type, ISettingsModule> modules = new();

        public UniTask Initialize() => UniTask.CompletedTask;

        public T GetModule<T>() where T : ISettingsModule
        {
            if (modules.TryGetValue(typeof(T), out var module))
                return (T)module;
            return default;
        }

        public void RegisterModule<T>(ISettingsModule module) where T : ISettingsModule
        {
            modules.Add(typeof(T), module);
        }
    }
}