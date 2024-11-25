using System;
using Meditation.Core;
using Meditation.Core.Utils;
using TMPro;
using UnityEngine;

namespace Meditation.Ui
{
    public class BreathingSettingsPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI durationLabel;
        [SerializeField] private TextMeshProUGUI inhaleDurationLabel;
        [SerializeField] private TextMeshProUGUI afterInhaleHoldLabel;
        [SerializeField] private TextMeshProUGUI exhaleDurationLabel;
        [SerializeField] private TextMeshProUGUI afterExhaleHoldLabel;

        public void Set(IBreathingSettings breathingSettings)
        {
            Debug.Assert(breathingSettings != null);
            durationLabel.text = $"{DateTimeUtils.GetTime(TimeSpan.FromSeconds(breathingSettings.GetTotalTime()))}";
            inhaleDurationLabel.text = $"{breathingSettings.GetInhaleDuration()} sec";
            if (breathingSettings.GetAfterInhaleDuration() > 0)
            {
                afterInhaleHoldLabel.transform.parent.gameObject.SetActive(true);
                afterInhaleHoldLabel.text = $"{breathingSettings.GetAfterInhaleDuration()} sec";
            }
            else
            {
                afterInhaleHoldLabel.transform.parent.gameObject.SetActive(false);
            }
            exhaleDurationLabel.text = $"{breathingSettings.GetExhaleDuration()} sec";
            if (breathingSettings.GetAfterExhaleDuration() > 0)
            {
                afterExhaleHoldLabel.transform.parent.gameObject.SetActive(true);
                afterExhaleHoldLabel.text = $"{breathingSettings.GetAfterExhaleDuration()} sec";
            }
            else
            {
                afterExhaleHoldLabel.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}