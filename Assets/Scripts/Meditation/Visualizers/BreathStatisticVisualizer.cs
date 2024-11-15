using Meditation.Ui;
using TMPro;
using UnityEngine;

namespace Meditation.Visualizers
{
    public class BreathStatisticVisualizer : ABreathStatisticVisualizer
    {
        [SerializeField] private TextMeshProUGUI totalLabel;
        [SerializeField] private SmoothText smoothText;

        public override void Init(int totalBreaths)
        {
            totalLabel.text = totalBreaths.ToString();
            SetActual(0);
        }
        public override void SetActual(int actualBreaths)
        {
            smoothText.Set(actualBreaths.ToString());
        }
    }
}