using Meditation.Apis.Settings;
using OneDay.Core;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;

namespace Meditation.Ui
{
    public class SoundSection : MonoBehaviour
    {
        [SerializeField] private CToggle soundBeepToggle;
        [SerializeField] private CToggle soundVoiceFemaleToggle;
        [SerializeField] private CToggle soundVoiceMaleToggle;
       
        private ISoundSettingsModule settings;
        
        private void Awake()
        {
            soundBeepToggle.onChange.AddListener(OnSoundBeepChanged);
            soundVoiceFemaleToggle.onChange.AddListener(OnSoundFemaleChanged);
            soundVoiceMaleToggle.onChange.AddListener(OnSoundMaleChanged);
        }

        public void Initialize()
        {
            settings = ServiceLocator.Get<ISettingsApi>().GetModule<ISoundSettingsModule>();
            soundBeepToggle.SetOn(settings.BreathingSoundMode == Mode.Beep, false);
            soundVoiceFemaleToggle.SetOn(settings.BreathingSoundMode == Mode.Voice_Female, false);
            soundVoiceMaleToggle.SetOn(settings.BreathingSoundMode == Mode.Voice_Male, false);
        }

        private void OnSoundBeepChanged(bool isOn)
        {
            if (isOn)
            {
                settings.BreathingSoundMode = Mode.Beep;
                soundVoiceFemaleToggle.SetOn(false, false);
                soundVoiceMaleToggle.SetOn(false, false);
                soundBeepToggle.SetOn(true, false);
            }
            else
            {
                soundBeepToggle.SetOn(true, false);
            }
        }

        private void OnSoundMaleChanged(bool isOn)
        {
            if (isOn)
            {
                settings.BreathingSoundMode = Mode.Voice_Male; 
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
                settings.BreathingSoundMode = Mode.Voice_Female; 
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