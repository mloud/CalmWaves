using Cysharp.Threading.Tasks;
using Meditation.Apis.Audio;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;

namespace Meditation.Ui.Audio
{
    public class AudioButton : MonoBehaviour
    {
        public AudioChangedCallbacks AudioCallbacks { get; set; }
        
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private CToggle toggle;
        [SerializeField] private AudioVolumeChanger volumeChanger;
        [SerializeField] private CImage icon;
        
        private AudioDefinition definition;
        
        private void Awake() => 
            toggle.onChange.AddListener(OnSelectionChanged);

        public AudioButton Set(AudioDefinition definition, (bool isSelected, float volume) state)
        {
            this.definition = definition;
            nameLabel.text = definition.Name;
            toggle.SetOn(state.isSelected, false);
            volumeChanger.SetVisible(state.isSelected);
            volumeChanger.SetVolume(state.volume);
            volumeChanger.VolumeChange = OnVolumeChanged;

            if (!string.IsNullOrEmpty(definition.IconKey))
            {
                icon.gameObject.SetActive(true);
                icon.SetImage(definition.IconKey).Forget();
            }
            else
            {
                icon.gameObject.SetActive(false);
            }
            return this;
        }

        private void OnVolumeChanged(float normalizedVolume) => 
            AudioCallbacks?.VolumeChanged?.Invoke(normalizedVolume, definition);

        private void OnSelectionChanged(bool isOn)
        {
            volumeChanger.SetVisible(isOn);
            AudioCallbacks?.SelectionChanged?.Invoke(isOn, definition);
        }
    }
}