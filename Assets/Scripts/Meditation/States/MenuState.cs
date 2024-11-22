using System;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Ui;
using Meditation.Ui.Views;

namespace Meditation.States
{
    public class MenuState : AState
    {
        private MenuView menuView;
        private AddressableAsset<BreathingSettingsDb> breathingDbAsset;
        private IBreathingApi breathingApi;
        public override async UniTask Initialize()
        {
            breathingApi = ServiceLocator.Get<IBreathingApi>();
            breathingDbAsset = await ServiceLocator.Get<IDataManager>().GetBreathingSettings();

            menuView = ServiceLocator.Get<IUiManager>().GetView<MenuView>();
            await menuView.InitializeBreathingButtons(breathingDbAsset.GetReference().GetAll() , OnMenuButtonClicked);
            await menuView.InitializeWeekCalendar(breathingApi.History.GetBreathingTimesThisWeek(), breathingApi.GetRequiredBreathingDuration());
            await menuView.InitializeTimer();

            menuView
                .BindAction(menuView.StartButton, OnStartClick)
                .BindAction(menuView.AiButton, OnAiClicked)
                .BindAction(menuView.MeasuringButton, OnMeasureClicked);
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            if (stateData != null && stateData.GetValue<bool>("FadeSkybox"))
            {
                menuView.FadeInSkybox().Forget();
            }
            await menuView.Show(true);
            if (stateData != null)
            {
                var finishedBreathing = stateData.GetValue<bool>(StateDataKeys.BreathingFinished);
                if (finishedBreathing)
                {
                    await menuView.ActualizeWeekCalendar(
                        DateTime.Today.DayOfWeek, 
                        breathingApi.History.GetBreathingTimeToday(),
                        breathingApi.GetRequiredBreathingDuration()
                        );
                }
            }
        }

        public override async UniTask ExecuteAsync()
        {
            
        }

        public override async UniTask ExitAsync()
        {
            await menuView.Hide(true);
        }
        
        private async UniTask OnStartClick()
        {
            var settings = await breathingApi.GetBreathingSettingsForCurrentPartOfTheDay();
            
            StateMachine.SetStateAsync<BreathingState>(
                    StateData.Create(("Settings", settings)), false)
                .Forget();
        }
      
        private void OnMenuButtonClicked(string id) =>
            StateMachine.SetStateAsync<BreathingState>(
                    StateData.Create(("Settings", breathingDbAsset.GetReference().Get(id))), false)
                .Forget();

        private async UniTask OnAiClicked() => 
            await StateMachine.SetStateAsync<MoodState>(waitForCurrentStateExit: false);
        
        private async UniTask OnMeasureClicked() => 
            await StateMachine.SetStateAsync<MeasuringState>(waitForCurrentStateExit: false);
    }
}