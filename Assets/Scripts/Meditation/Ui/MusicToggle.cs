using UnityEngine;

namespace Meditation.Ui
{
    public class MusicToggle : MonoBehaviour
    {
        [SerializeField] private CToggle toggle;

        private void Start()
        {
            toggle.SetOn(ServiceLocator.Get<IAudioManager>().MusicEnabled, false);
            toggle.onChange.AddListener(isOn => ServiceLocator.Get<IAudioManager>().MusicEnabled = isOn);
        }

        private void OnValidate()
        {
            toggle = GetComponent<CToggle>();
        }
    }
}