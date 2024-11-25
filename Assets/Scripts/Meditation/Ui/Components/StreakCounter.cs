using Meditation.Apis;
using Meditation.Ui.Text;
using OneDay.Core;
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