using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Core.Utils;
using Meditation.Ui.Chart;
using Meditation.Ui.Views;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Audio;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;

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
            await menuView.InitializeWeekCalendar(breathingApi.BreathingHistory.GetBreathingTimesThisWeek(), breathingApi.GetRequiredBreathingDuration());
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
            
            // set breathing chart
            var breathingTimesThisWeek = breathingApi.BreathingHistory.GetBreathingTimesThisWeek();
            var currentMaxInWeek = breathingTimesThisWeek.Max(x => x.Item2);
            var chartMax = currentMaxInWeek > breathingApi.GetRequiredBreathingDuration()
                ? currentMaxInWeek
                : breathingApi.GetRequiredBreathingDuration();
            
            menuView.BreathingChart.Name = "Week breathing";
            menuView.BreathingChart.Units = "";
            menuView.BreathingChart.ValueToStringConversion = DateTimeUtils.GetTime;
            menuView.BreathingChart.Set(new DayTimeSpanChartData(breathingTimesThisWeek, chartMax));
            menuView.BreathingChart.Select(DateTime.Now.DayOfWeek);
            
            await menuView.Show(true);
            ServiceLocator.Get<IAudioManager>().PlayMusic("Menu");
            if (stateData != null)
            {
                var finishedBreathing = stateData.GetValue<bool>(StateDataKeys.BreathingFinished);
                if (finishedBreathing)
                {
                    await menuView.ActualizeWeekCalendar(
                        DateTime.Today.DayOfWeek, 
                        breathingApi.BreathingHistory.GetBreathingTimeToday(),
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