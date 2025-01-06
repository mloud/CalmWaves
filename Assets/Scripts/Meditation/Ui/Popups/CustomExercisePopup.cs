using System;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Data;
using Meditation.Ui.Components;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class CustomExercisePopup : UiPopup
    {
        public UserBreathingSettings BreathingSettings => breathingSettings;
        public Button SaveButton => saveButton;
        
        [SerializeField] private Button saveButton;
        [SerializeField] private Button nextButton;

        [SerializeField] private TextMeshProUGUI largeNameLabel;
        [SerializeField] private IntValueChanger inhaleValueChanger;
        [SerializeField] private IntValueChanger exhaleValueChanger;
        [SerializeField] private IntValueChanger afterInhaleHoldChanger;
        [SerializeField] private IntValueChanger afterExhaleHoldChanger;
        [SerializeField] private IntValueChanger roundsChanger;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TimeSpanText timeSpanText;
        [SerializeField] private GameObject settingsNameStateContainer;
        [SerializeField] private GameObject settingsTimerStateContainer;
        [SerializeField] private AExtendedText inhaleInfoLabel;
        [SerializeField] private AExtendedText holdAfterInhaleInfoLabel;
        [SerializeField] private AExtendedText exhaleInfoLabel;
        [SerializeField] private AExtendedText holdAfterExhaleInfoLabel;

        private UserBreathingSettings breathingSettings;

        private enum State
        {
            ChoosingName,
            SettingsTimer
        }

        private State state;
        protected override void OnInit()
        {
            inhaleValueChanger.Initialize(4, 1,15);
            exhaleValueChanger.Initialize(4, 1,15);
            afterInhaleHoldChanger.Initialize(4, 0,10);
            afterExhaleHoldChanger.Initialize(4, 0,10);
            roundsChanger.Initialize(12, 1,100);

            inputField.onValueChanged.AddListener(OnNameChanged);
            inputField.text = "";
            inhaleValueChanger.OnValueChanged += async (v) =>
            {
                breathingSettings.BreathingTiming.InhaleDuration = v;
                await RefreshInhaleDescription();
            };
            exhaleValueChanger.OnValueChanged += async v =>
            {
                breathingSettings.BreathingTiming.ExhaleDuration = v;
                await RefreshExhaleDescription();
            };
            afterInhaleHoldChanger.OnValueChanged += async v =>
            {
                breathingSettings.BreathingTiming.AfterInhaleDuration = v;
                await RefreshHoldAfterInhaleDescription();
            };
            afterExhaleHoldChanger.OnValueChanged += async v =>
            {
                breathingSettings.BreathingTiming.AfterExhaleDuration = v;
                await RefreshHoldAfterExhaleDescription();
            };
            roundsChanger.OnValueChanged += (v) => breathingSettings.Rounds = v;
            nextButton.onClick.AddListener(async ()=>await OnNext());
            UpdateNextButton();
        }

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            breathingSettings = UserBreathingSettings.Default();
            inhaleValueChanger.Set((int)breathingSettings.GetInhaleDuration());
            exhaleValueChanger.Set((int)breathingSettings.GetExhaleDuration());
            afterInhaleHoldChanger.Set((int)breathingSettings.GetAfterInhaleDuration());
            afterExhaleHoldChanger.Set((int)breathingSettings.GetAfterExhaleDuration());
            roundsChanger.Set(breathingSettings.Rounds);
            timeSpanText.Set(TimeSpan.FromSeconds(breathingSettings.GetTotalTime()));
            await EnterState(State.ChoosingName);
                
            ServiceLocator.Get<IUiManager>().HideView();
        }
     
        protected override UniTask OnCloseStarted()
        {
            ServiceLocator.Get<IUiManager>().ShowView();
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
            UpdateNextButton();
            breathingSettings.Name = name;
        }
        
        private void UpdateNextButton()=> nextButton.interactable = inputField.text.Length > 0;

        private void Update()
        {
           UpdateState(state);
        }

        private async UniTask EnterState(State state)
        {
            switch (state)
            {
                case State.ChoosingName:
                    inputField.text = "";
                    await settingsNameStateContainer.SetVisibleWithFade(true, 0, true);
                    await settingsTimerStateContainer.SetVisibleWithFade(false, 0, true);
                    break;
                
                case State.SettingsTimer:
                    await UniTask.WhenAll(
                        RefreshHoldAfterExhaleDescription(),
                        RefreshHoldAfterInhaleDescription(),
                        RefreshExhaleDescription(),
                        RefreshInhaleDescription());
                    await settingsNameStateContainer.SetVisibleWithFade(false, 0.2f, true);
                    await settingsTimerStateContainer.SetVisibleWithFade(true, 1.0f, true);
                    break;
            }

            this.state = state;
        }

        private void UpdateState(State state)
        {
            switch (state)
            {
                case State.ChoosingName:
                  
                    break;
                case State.SettingsTimer:
                    timeSpanText.Set(TimeSpan.FromSeconds(breathingSettings.GetTotalTime()));
                    break;
            }
        }
        
        private async UniTask OnNext()
        {
            largeNameLabel.text = inputField.text;
            await EnterState(State.SettingsTimer);
        }

        private async UniTask RefreshInhaleDescription() => 
            inhaleInfoLabel.text = await ServiceLocator.Get<IBreathingApi>()
                .GetBreathingPhaseDescription("inhale", breathingSettings.BreathingTiming.InhaleDuration);

        private async UniTask RefreshExhaleDescription() => 
            exhaleInfoLabel.text = await ServiceLocator.Get<IBreathingApi>()
                .GetBreathingPhaseDescription("exhale", breathingSettings.BreathingTiming.ExhaleDuration);

        private async UniTask RefreshHoldAfterInhaleDescription() => 
            holdAfterInhaleInfoLabel.text = await ServiceLocator.Get<IBreathingApi>()
                .GetBreathingPhaseDescription("holdAfterInhale", breathingSettings.BreathingTiming.AfterInhaleDuration);
        private async UniTask RefreshHoldAfterExhaleDescription() => 
            holdAfterExhaleInfoLabel.text = await ServiceLocator.Get<IBreathingApi>()
                .GetBreathingPhaseDescription("holdAfterExhale", breathingSettings.BreathingTiming.AfterExhaleDuration);

    }
}