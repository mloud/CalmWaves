using OneDay.Core;
using UnityEngine;

namespace Meditation.Ui
{
    public class MusicSection : MonoBehaviour
    {
        [SerializeField] private CToggle musicOnToggle;
        [SerializeField] private CToggle musicOffToggle;

        private void Awake()
        {
            musicOnToggle.onChange.AddListener(OnMusicOnChanged);
            musicOffToggle.onChange.AddListener(OnMusicOffChanged);
        }

        public void Initialize()
        {
            musicOnToggle.SetOn(ServiceLocator.Get<IAudioManager>().MusicEnabled, false); 
            musicOffToggle.SetOn(!ServiceLocator.Get<IAudioManager>().MusicEnabled, false); 
        }

        private void OnMusicOnChanged(bool isOn)
        {
            musicOffToggle.SetOn(!isOn, false);
            ServiceLocator.Get<IAudioManager>().MusicEnabled = isOn;
        }
        
        private void OnMusicOffChanged(bool isOn)
        {
            musicOnToggle.SetOn(!isOn, false);
            ServiceLocator.Get<IAudioManager>().MusicEnabled = !isOn;
        }
    }
}