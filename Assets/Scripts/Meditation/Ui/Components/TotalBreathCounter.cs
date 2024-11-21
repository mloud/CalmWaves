using Meditation.Apis;
using Meditation.Ui.Text;
using UnityEngine;

namespace Meditation.Ui.Components
{
    public class TotalBreathCounter : MonoBehaviour
    {
        [SerializeField] private AExtendedText counterLabel;
        
        public void Initialize()
        {
            var breathingApi = ServiceLocator.Get<IBreathingApi>();
            counterLabel.Set(breathingApi.History.GetTotalBreathCyclesCount().ToString());
            breathingApi.TotalBreathCountChanged += OnTotalBreathsCountChanged;
        }

        private void OnTotalBreathsCountChanged(int count) =>
            counterLabel.Set(count.ToString());

        private void OnDestroy() =>
            ServiceLocator.Get<IBreathingApi>().TotalBreathCountChanged -= OnTotalBreathsCountChanged;
    }
}