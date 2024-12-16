using Meditation.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class SleepView : UiView
    {
        public Button AudioButton => audioButton;
        public IntValueChanger MinutesValueChanger => minutesValueChanger;
        public IntValueChanger HoursValueChanger => hoursValueChanger;
  
        [SerializeField] private Button audioButton;
        [SerializeField] private IntValueChanger minutesValueChanger;
        [SerializeField] private IntValueChanger hoursValueChanger;
    }
}