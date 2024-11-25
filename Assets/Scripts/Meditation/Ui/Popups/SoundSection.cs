using Meditation.Apis.Settings;
using OneDay.Core;
using UnityEngine;

namespace Meditation.Ui
{
    public class SoundSection : MonoBehaviour
    {
        [SerializeField] private CToggle soundBeepToggle;
        [SerializeField] private CToggle soundVoiceFemaleToggle;
        [SerializeField] private CToggle soundVoiceMaleToggle;
        [SerializeField] private CToggle soundOffToggle;

        private ISoundSettingsModule settings;
        
        private void Awake()
        {
            soundBeepToggle.onChange.AddListener(OnSoundBeepChanged);
            soundVoiceFemaleToggle.onChange.AddListener(OnSoundFemaleChanged);
            soundVoiceMaleToggle.onChange.AddListener(OnSoundMaleChanged);
            soundOffToggle.onChange.AddListener(OnSoundOffChanged);
        }

        public void Initialize()
        {
            settings = ServiceLocator.Get<ISettingsApi>().GetModule<ISoundSettingsModule>();
            if (ServiceLocator.Get<IAudioManager>().SfxEnabled)
            {
                soundOffToggle.SetOn(false, false);
                soundBeepToggle.SetOn(settings.BreathingSoundMode == Mode.Beep, false);
                soundVoiceFemaleToggle.SetOn(settings.BreathingSoundMode == Mode.Voice_Female, false);
                soundVoiceMaleToggle.SetOn(settings.BreathingSoundMode == Mode.Voice_Male, false);
            }
            else
            {
                soundVoiceMaleToggle.SetOn(false, false);
                soundBeepToggle.SetOn(false, false);
                soundVoiceFemaleToggle.SetOn(false, false);
                soundOffToggle.SetOn(true, false);
            }
        }
        
        private void OnSoundBeepChanged(bool isOn)
        {
            if (isOn)
            {
                ServiceLocator.Get<IAudioManager>().SfxEnabled = true;
                settings.BreathingSoundMode = Mode.Beep;
                soundOffToggle.SetOn(false, false);
                soundVoiceFemaleToggle.SetOn(false, false);
                soundVoiceMaleToggle.SetOn(false, false);
                soundBeepToggle.SetOn(true, false);
            }
            else
            {
                soundBeepToggle.SetOn(true, false);
            }
        }


        private void OnSoundOffChanged(bool isOn)
        {
            if (isOn)
            {
                ServiceLocator.Get<IAudioManager>().SfxEnabled = false;
                soundOffToggle.SetOn(true, false);
                soundVoiceFemaleToggle.SetOn(false, false);
                soundVoiceMaleToggle.SetOn(false, false);
                soundBeepToggle.SetOn(false, false);
            }
            else
            {
                soundOffToggle.SetOn(true, false);
            }
        }

        private void OnSoundMaleChanged(bool isOn)
        {
            if (isOn)
            {
                ServiceLocator.Get<IAudioManager>().SfxEnabled = true;
                settings.BreathingSoundMode = Mode.Voice_Male; 
                soundOffToggle.SetOn(false, false);
                soundVoiceFemaleToggle.SetOn(false, false);
                soundVoiceMaleToggle.SetOn(true, false);
                soundBeepToggle.SetOn(false, false);
            }
            else
            {
                soundVoiceMaleToggle.SetOn(true, false);
            }
        }

        private void OnSoundFemaleChanged(bool isOn)
        {
            if (isOn)
            {
                ServiceLocator.Get<IAudioManager>().SfxEnabled = true;
                settings.BreathingSoundMode = Mode.Voice_Female; 
                soundOffToggle.SetOn(false, false);
                soundVoiceFemaleToggle.SetOn(true, false);
                soundVoiceMaleToggle.SetOn(false, false);
                soundBeepToggle.SetOn(false, false);
            }
            else
            {
                soundVoiceFemaleToggle.SetOn(true, false);
            }
        }
    }
}