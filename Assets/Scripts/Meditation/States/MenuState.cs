using System;
using System.Linq;
using System.Threading;
using Core.Modules.Ui.Effects;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Audio;
using Meditation.Apis.Visual;
using Meditation.Core.Utils;
using Meditation.Data;
using Meditation.Data.Breathing;
using Meditation.Ui;
using Meditation.Ui.Audio;
using Meditation.Ui.Chart;
using Meditation.Ui.Views;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Assets;
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
        private IJourneyManager journeyManager;

        private CancellationTokenSource cancellationTokenSource;
        
        public override async UniTask Initialize()
        {
            breathingApi = ServiceLocator.Get<IBreathingApi>();
            breathingDbAsset = await ServiceLocator.Get<IDataManager>().GetBreathingSettings();
            uiManager = ServiceLocator.Get<IUiManager>();
            dataManager = ServiceLocator.Get<IDataManager>();
            journeyManager = ServiceLocator.Get<IJourneyManager>();
            
            menuView = ServiceLocator.Get<IUiManager>().GetView<MenuView>();
            await menuView.InitializeBreathingButtons(breathingDbAsset.GetReference().GetAll() , OnMenuButtonClicked);
            await menuView.InitializeWeekCalendar(breathingApi.BreathingHistory.GetBreathingTimesThisWeek(), breathingApi.GetRequiredBreathingDuration());
            await menuView.InitializeTimer();
            await menuView.InitializePremium();

            menuView
                .BindAction(menuView.StartButton, OnStartClick)
                .BindAction(menuView.AiButton, OnAiClicked)
                .BindAction(menuView.MeasuringButton, OnMeasureClicked)
                .BindAction(menuView.CustomExerciseContainer.CreateNewButton, OnCreateNewExercise)
                .BindAction(menuView.MusicButton, OnMusicClicked)
                .BindAction(menuView.SleepButton, OnSleepClicked)
                .BindAction(menuView.SettingsButton, OnSettingsClicked)
                .BindAction(menuView.JourneyButton, OnJourneyClicked);

            
    
            await menuView.CustomExerciseContainer.Initialize(await dataManager.GetAll<UserBreathingSettings>());
            menuView.CustomExerciseContainer.BreathingSettingsSelected += OnCustomBreathingClicked;
            menuView.CustomExerciseContainer.BreathingSettingsDeleted += async (s) => await OnCustomBreathingDeleteClicked(s);
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            if (stateData != null && stateData.GetValue<bool>("FadeSkybox"))
            {
                await ServiceLocator.Get<IVisualEnvironmentManager>().FadeInSkybox();
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
            var journeyProgression = (await journeyManager.GetProgression("LungCapacity20")) ??
                                     JourneyProgression.First("LungCapacity20");
            var journeyDefinition = await journeyManager.GetJourney("LungCapacity20");
            menuView.JourneyProgression.Set(journeyProgression.CurrentProgress + 1, journeyDefinition.GetMissions().Count());
            ServiceLocator.Get<IUiManager>().GetAllPanels().ForEach(view=>view.Show(true));
            await menuView.Show(true);
            menuView.MessageComponent.StartDisplaying();
            
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

            PlayParticleEffectsOnUi(cancellationTokenSource.Token);
            PlayFloatingEffects(cancellationTokenSource.Token);
            ServiceLocator.Get<IAudioEnvironmentManager>().Apply("default");
        }

        public override async UniTask ExecuteAsync()
        {
            
        }

        public override async UniTask ExitAsync()
        {
            cancellationTokenSource.Cancel();
            await menuView.Hide(true);
            menuView.MessageComponent.StopDisplaying();
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
                await menuView.CustomExerciseContainer.Initialize(await dataManager.GetAll<UserBreathingSettings>());
                request.Popup.Close().Forget();
                
            }, true);

            await request.OpenTask;
        }
        
        private void OnCustomBreathingClicked(UserBreathingSettings settings)
        {
            StateMachine.SetStateAsync<BreathingState>(
                    StateData.Create(("Settings", settings)), false)
                .Forget();
        }

        private async UniTask OnCustomBreathingDeleteClicked(UserBreathingSettings settings)
        {
            var id = (await dataManager.GetAll<UserBreathingSettings>())
                .First(x => x.CreateTime == settings.CreateTime).Id;
            await dataManager.Remove<UserBreathingSettings>(id);
            await menuView.CustomExerciseContainer.Initialize(await dataManager.GetAll<UserBreathingSettings>());
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
        
        private async UniTask OnMusicClicked()
        {
            var request = uiManager.OpenPopup<AudioPopup>(null);
            await request.OpenTask;
        }
        
        private void OnSleepClicked()
        {
            StateMachine.SetStateAsync<SleepState>(null, false)
                .Forget();
        }

        private async UniTask OnSettingsClicked()
        {
            var request = uiManager.OpenPopup<MenuSettingsPopup>(null);
            await request.OpenTask;
        }
        
        private void OnJourneyClicked()
        {
            StateMachine.SetStateAsync<JourneyState>(null, false)
                .Forget();
        }
        
        private async UniTask PlayFloatingEffects(CancellationToken cancellationToken)
        {
            var effects = ServiceLocator.Get<IEffectManager>()
                .GetEffects("FloatingEffect");
            effects.ForEach(x=>x.Run());

            effects = ServiceLocator.Get<IEffectManager>()
                .GetEffects("FloatingButtonEffect");
            foreach (var effect in effects)
            {
                effect.Run();
                await UniTask.WaitForSeconds(0.8f,cancellationToken:cancellationToken);
            }
        }
        
        private async UniTask PlayParticleEffectsOnUi(CancellationToken cancellationToken)
        {
            var effects = ServiceLocator.Get<IEffectManager>()
                .GetEffects("UiEdgeParticle").ToList();
            
            if (effects.Count == 0)
                return;

            var randomizedEffects = effects.GetRandomized();
       
            int index = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await UniTask.WaitForSeconds(5.0f, cancellationToken: cancellationToken);
                    var effect = randomizedEffects[index];
                    effect.Run();
                    index = (index + 1) % effects.Count;

                    await UniTask.WaitUntil(() => !effect.IsPlaying(), cancellationToken: cancellationToken);
                }
                catch (OperationCanceledException _)
                {
                }
            }
        }
    }
}