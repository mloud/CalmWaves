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

        private void OnDestroy() => audioManager.MusicStateChanged -= OnMusicStateChanged;

        private void OnMusicStateChanged(bool isMusicEnabled) => toggle.SetOn(isMusicEnabled, false);

        private void OnValidate()
        {
            toggle = GetComponent<CToggle>();
        }
    }
}