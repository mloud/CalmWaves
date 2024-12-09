using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Core.Utils;
using Meditation.Data;
using Meditation.Ui;
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
        private IDataManager dataManager;
        private IBreathingApi breathingApi;
        private IUiManager uiManager;
        
        public override async UniTask Initialize()
        {
            breathingApi = ServiceLocator.Get<IBreathingApi>();
            breathingDbAsset = await ServiceLocator.Get<IDataManager>().GetBreathingSettings();
            uiManager = ServiceLocator.Get<IUiManager>();
            dataManager = ServiceLocator.Get<IDataManager>();
            
            menuView = ServiceLocator.Get<IUiManager>().GetView<MenuView>();
            await menuView.InitializeBreathingButtons(breathingDbAsset.GetReference().GetAll() , OnMenuButtonClicked);
            await menuView.InitializeWeekCalendar(breathingApi.BreathingHistory.GetBreathingTimesThisWeek(), breathingApi.GetRequiredBreathingDuration());
            await menuView.InitializeTimer();

            menuView
                .BindAction(menuView.StartButton, OnStartClick)
                .BindAction(menuView.AiButton, OnAiClicked)
                .BindAction(menuView.MeasuringButton, OnMeasureClicked)
                .BindAction(menuView.CustomExerciseContainer.CreateNewButton, OnCreateNewExercise)
                .BindAction(menuView.NotificationButton, OnNotificationClicked)
                .BindAction(menuView.SubscriptionButton, OnSubscriptionClicked);
    
            await menuView.CustomExerciseContainer.Initialize(await dataManager.GetAll<CustomBreathingSettings>());
            menuView.CustomExerciseContainer.BreathingSettingsSelected += OnCustomBreathingClicked;
            menuView.CustomExerciseContainer.BreathingSettingsDeleted += async (s) => await OnCustomBreathingDeleteClicked(s);
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
            menuView.CustomExerciseContainer.ScrollTobBeginning();
            
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

        private async UniTask OnCreateNewExercise()
        {
            var request = uiManager.OpenPopup<CustomExercisePopup>(null);
            request.Popup.BindAction(request.Popup.SaveButton, async () =>
            {
                var settings = request.Popup.BreathingSettings;
                await dataManager.Add(settings);
                await menuView.CustomExerciseContainer.Initialize(await dataManager.GetAll<CustomBreathingSettings>());
                request.Popup.Close().Forget();
                
            }, true);

            await request.OpenTask;
        }
        
        private void OnCustomBreathingClicked(CustomBreathingSettings settings)
        {
            StateMachine.SetStateAsync<BreathingState>(
                    StateData.Create(("Settings", settings)), false)
                .Forget();
        }

        private async UniTask OnCustomBreathingDeleteClicked(CustomBreathingSettings settings)
        {
            var id = (await dataManager.GetAll<CustomBreathingSettings>())
                .First(x => x.CreateTime == settings.CreateTime).Id;
            await dataManager.Remove<CustomBreathingSettings>(id);
            await menuView.CustomExerciseContainer.Initialize(await dataManager.GetAll<CustomBreathingSettings>());
        }
        
        private async UniTask OnNotificationClicked()
        {
            var request = uiManager.OpenPopup<NotificationPopup>(null);
            await request.OpenTask;
        }
        
        private async UniTask OnSubscriptionClicked()
        {
            var request = uiManager.OpenPopup<SubscriptionPopup>(null);
            await request.OpenTask;
        }
    }
}