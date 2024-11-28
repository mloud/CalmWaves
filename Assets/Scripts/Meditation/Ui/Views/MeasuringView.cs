using Meditation.Ui.Components;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class MeasuringView : UiView
    {
        public GameObject MeasureCircle => measureCircle;
        public GameObject TapToStartCircle => tapToStartCircle;
        public GameObject Result => result;
        public MousePressReleaseHandler MouseHandler => mouseHandler;
        public SmoothText Prompt => promptLabel;
        public TextMeshProUGUI TimerLabel => timerLabel;
        public TextMeshProUGUI SubtitleLabel => subtitleLabel;
        
        public SmoothText TitleLabel => titleLabel;

        public Button SaveButton => saveButton;
        public Button MeasuringButton => measuringButton;
        
        public TextMeshProUGUI ExhaleValue => exhaleValue;
        public TextMeshProUGUI InhaleValue => inValue;
    
        [SerializeField] private SmoothText promptLabel;
        [SerializeField] private TextMeshProUGUI timerLabel;
        [SerializeField] private TextMeshProUGUI subtitleLabel;
        [SerializeField] private TextMeshProUGUI exhaleValue;
        [SerializeField] private TextMeshProUGUI inValue;
        [SerializeField] private SmoothText titleLabel;
        [SerializeField] private GameObject measureCircle;
        [SerializeField] private GameObject tapToStartCircle;
        [SerializeField] private GameObject result;
        [SerializeField] private MousePressReleaseHandler mouseHandler;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button measuringButton;
    }
}