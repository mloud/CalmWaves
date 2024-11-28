using OneDay.Core;
using OneDay.Core.Modules.Audio;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;

namespace Meditation.Ui
{
    public class MusicToggle : MonoBehaviour
    {
        [SerializeField] private CToggle toggle;

        private IAudioManager audioManager;
        private void Start()
        {
            audioManager = ServiceLocator.Get<IAudioManager>();
            toggle.SetOn(audioManager.MusicEnabled, false);
            toggle.onChange.AddListener(isOn => audioManager.MusicEnabled = isOn);

            audioManager.MusicStateChanged += OnMusicStateChanged;
        }

        private void OnDestroy()
        {
            if (audioManager != null)
                audioManager.MusicStateChanged -= OnMusicStateChanged;
        }

        private void OnMusicStateChanged(bool isMusicEnabled) => toggle.SetOn(isMusicEnabled, false);

        private void OnValidate()
        {
            toggle = GetComponent<CToggle>();
        }
    }
}