using Meditation.Apis;
using TMPro;
using UnityEngine;

namespace Meditation.Ui.Components
{
    public class TotalBreathCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI counterLabel;
        
        public void Initialize()
        {
            var breathingApi = ServiceLocator.Get<IBreathingApi>();
            counterLabel.text = breathingApi.History.GetTotalBreathCyclesCount().ToString();
            breathingApi.TotalBreathCountChanged += OnTotalBreathsCountChanged;
        }

        private void OnTotalBreathsCountChanged(int count) => 
            counterLabel.text = count.ToString();

        private void OnDestroy() =>
            ServiceLocator.Get<IBreathingApi>().TotalBreathCountChanged -= OnTotalBreathsCountChanged;
    }
}