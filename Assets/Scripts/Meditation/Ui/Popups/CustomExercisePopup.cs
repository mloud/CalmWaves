using System;
using Cysharp.Threading.Tasks;
using Meditation.Data;
using Meditation.Ui.Components;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class CustomExercisePopup : UiPopup
    {
        public CustomBreathingSettings BreathingSettings => breathingSettings;
        public Button SaveButton => saveButton;
        
        [SerializeField] private Button saveButton;
        [SerializeField] private IntValueChanger inhaleValueChanger;
        [SerializeField] private IntValueChanger exhaleValueChanger;
        [SerializeField] private IntValueChanger afterInhaleHoldChanger;
        [SerializeField] private IntValueChanger afterExhaleHoldChanger;
        [SerializeField] private IntValueChanger roundsChanger;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TimeSpanText timeSpanText;

        private CustomBreathingSettings breathingSettings;
        protected override void OnInit()
        {
            inhaleValueChanger.Initialize(4, 1,15);
            exhaleValueChanger.Initialize(4, 1,15);
            afterInhaleHoldChanger.Initialize(4, 0,10);
            afterExhaleHoldChanger.Initialize(4, 0,10);
            roundsChanger.Initialize(12, 1,100);

            inputField.onValueChanged.AddListener(OnNameChanged);
            inputField.text = "";
            inhaleValueChanger.OnValueChanged += (v) => breathingSettings.BreathingTiming.InhaleDuration = v;
            exhaleValueChanger.OnValueChanged += (v) => breathingSettings.BreathingTiming.ExhaleDuration = v;
            afterInhaleHoldChanger.OnValueChanged += (v) => breathingSettings.BreathingTiming.AfterInhaleDuration = v;
            afterExhaleHoldChanger.OnValueChanged += (v) => breathingSettings.BreathingTiming.AfterExhaleDuration = v;
            roundsChanger.OnValueChanged += (v) => breathingSettings.Rounds = v;
            
            UpdateSaveButton();
        }

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            breathingSettings = CustomBreathingSettings.Default();
            inputField.text = "";
            inhaleValueChanger.Set((int)breathingSettings.GetInhaleDuration());
            exhaleValueChanger.Set((int)breathingSettings.GetExhaleDuration());
            afterInhaleHoldChanger.Set((int)breathingSettings.GetAfterInhaleDuration());
            afterExhaleHoldChanger.Set((int)breathingSettings.GetAfterExhaleDuration());
            roundsChanger.Set(breathingSettings.Rounds);
                
            ServiceLocator.Get<IUiManager>().HideRootView();
        }
     
        protected override UniTask OnCloseStarted()
        {
            ServiceLocator.Get<IUiManager>().ShowRootViews();
            return UniTask.CompletedTask;
        }
        
        protected override UniTask OnOpenFinished(IUiParameter parameter)
        {
            return UniTask.CompletedTask;
        }
        
        private void OnNameChanged(string name)
        {
            name = name[..Math.Min(15, name.Length)];
            inputField.text = name;
            UpdateSaveButton();
            breathingSettings.Name = name;
        }
        
        private void UpdateSaveButton()=> SaveButton.interactable = inputField.text.Length > 0;

        private void Update()
        {
            if (breathingSettings == null)
                return;
            
            timeSpanText.Set(TimeSpan.FromSeconds(breathingSettings.GetTotalTime()));
        }
    }
}