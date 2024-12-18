using Meditation.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class SleepView : UiView
    {
        public GameObject SettingsContainer => settingsContainer;
        public GameObject RunningContainer => runningContainer;
        public GameObject FinishedContainer => finishedContainer;
        public Button AudioButton => audioButton;
        public Button ContinueButton => continueButton;
        public IntValueChanger MinutesValueChanger => minutesValueChanger;
        public IntValueChanger HoursValueChanger => hoursValueChanger;
        public IntValueChanger SecondsValueChanger => secondsValueChanger;
        public GameObject TimerContainer => timerContainer;

        [SerializeField] private Button audioButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private IntValueChanger secondsValueChanger;
        [SerializeField] private IntValueChanger minutesValueChanger;
        [SerializeField] private IntValueChanger hoursValueChanger;
        [SerializeField] private GameObject settingsContainer;
        [SerializeField] private GameObject runningContainer;
        [SerializeField] private GameObject finishedContainer;
        [SerializeField] private GameObject timerContainer;
    }
}