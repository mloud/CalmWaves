using OneDay.Core;

namespace Meditation.Ui
{
    using UnityEngine;

    namespace Meditation.Ui
    {
        public class SfxToggle : MonoBehaviour
        {
            [SerializeField] private CToggle toggle;

            private void Start()
            {
                toggle.SetOn(ServiceLocator.Get<IAudioManager>().SfxEnabled, false);
                toggle.onChange.AddListener(isOn => ServiceLocator.Get<IAudioManager>().SfxEnabled = isOn);
            }

            private void OnValidate()
            {
                toggle = GetComponent<CToggle>();
            }
        }
    }
}