using Meditation.Apis;
using OneDay.Core;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;

namespace Meditation.Ui.Components
{
    public class StreakCounter : MonoBehaviour
    {
        [SerializeField] private AExtendedText counterLabel;
        
        public void Initialize()
        {
            var breathingApi = ServiceLocator.Get<IBreathingApi>();
            counterLabel.Set(breathingApi.GetStreak().ToString());
            breathingApi.StreakCountChanged += OnStreakChanged;
        }

        private void OnStreakChanged(int count) =>
            counterLabel.Set(count.ToString());

        private void OnDestroy() =>
            ServiceLocator.Get<IBreathingApi>().StreakCountChanged -= OnStreakChanged;
    }
}