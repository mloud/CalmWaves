using Meditation.Ui.Components;
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
        public TextMeshProUGUI Prompt => promptLabel;
        public TextMeshProUGUI TimerLabel => timerLabel;
        public Button SaveButton => saveButton;
        
        public TextMeshProUGUI ExhaleValue => exhaleValue;
        public TextMeshProUGUI InhaleValue => inValue;
    
        [SerializeField] private TextMeshProUGUI promptLabel;
        [SerializeField] private TextMeshProUGUI timerLabel;
        [SerializeField] private TextMeshProUGUI exhaleValue;
        [SerializeField] private TextMeshProUGUI inValue;
        [SerializeField] private GameObject measureCircle;
        [SerializeField] private GameObject tapToStartCircle;
        [SerializeField] private GameObject result;
        [SerializeField] private MousePressReleaseHandler mouseHandler;
        [SerializeField] private Button saveButton;
    }
}