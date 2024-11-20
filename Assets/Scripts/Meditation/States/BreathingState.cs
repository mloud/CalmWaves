using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Settings;
using Meditation.Ui.Views;
using UnityEngine;

namespace Meditation.States
{
    public class BreathingState : AState
    {
        private CancellationTokenSource cancellationTokenSource;
        private ISoundSettingsModule settings;
        private IAudioManager audioManager;
        private IUpdateManager updateManager;
        private IUiManager uiManager;
        private IBreathingApi breathingApi;
        private IBreathingSettings breathingSettings;
        private BreathingView breathingView;
        
        private AddressableAsset<AudioClip> winClip;
        private bool breathingFinished;
        private bool running;
        private bool paused;
        private float breathingTime;
        
        
        public override async UniTask Initialize()
        {
            audioManager = ServiceLocator.Get<IAudioManager>();
            updateManager = ServiceLocator.Get<IUpdateManager>();
            uiManager = ServiceLocator.Get<IUiManager>();
            breathingApi = ServiceLocator.Get<IBreathingApi>();

            settings = ServiceLocator.Get<ISettingsApi>().GetModule<ISoundSettingsModule>();
            breathingView = uiManager.GetView<BreathingView>();
            breathingView
                .BindAction(breathingView.PauseButton, OnPause)
                .BindAction(breathingView.BackButton, OnBack)
                .BindAction(breathingView.SettingsButton, OnSettings);
           
            winClip = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<AudioClip>("win");
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            breathingTime = 0;
            breathingFinished = false;
            Debug.Assert(stateData != null, "State data is required when entering BreathingState");
            breathingSettings = stateData.GetValue<IBreathingSettings>("Settings");
         
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            // initialize
            breathingView.TotalTimeVisualizer.Initialize();
            breathingView.BreathingVisualizer.Initialize();
            breathingView.TextFader.Clear();
            breathingView.PauseText.Clear();
            breathingView.NameLabel.text = breathingSettings.GetName();
            breathingView.BreathStatisticVisualizer.Init(breathingSettings.Rounds);

            try
            {
                // intro
                await breathingView.Show(false);
                if (!cancellationTokenSource.IsCancellationRequested) 
                    await breathingView.HideElements();
                
                if (!cancellationTokenSource.IsCancellationRequested) 
                    await uiManager.ShowInfoPopup(breathingSettings, false, false);
                
                await breathingView.CountDownVisualizer.Run(
                    cancellationTokenSource.Token, () => uiManager.HideInfoPopup());

                if (!cancellationTokenSource.IsCancellationRequested) 
                    await breathingView.FadeInElements();
                
                audioManager.PlayMusic(breathingSettings.GetMusic()).Forget();

                // breathing
                if (!cancellationTokenSource.IsCancellationRequested)
                    await CountBreathing(cancellationTokenSource.Token);

                if (!cancellationTokenSource.IsCancellationRequested) 
                    audioManager.PlaySfx(winClip.GetReference());
                
                if (!cancellationTokenSource.IsCancellationRequested)
                    await breathingApi.FinishSession(breathingSettings.GetTotalTime());
                
                if (!cancellationTokenSource.IsCancellationRequested) 
                    await breathingView.FadeOutElements();
                
                if (!cancellationTokenSource.IsCancellationRequested) 
                    await breathingView.TextFader.Show();
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Breathing was cancelled");
            }
            finally
            {
                Debug.Log("Finally");
            }
        }

        public override UniTask ExecuteAsync()
        {
            return UniTask.CompletedTask;
        }

        public override async UniTask ExitAsync()
        {
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
            ServiceLocator.Get<IAudioManager>().StopMusic();
            await breathingView.Hide(true);
        }
        
        private async UniTask<bool> CountBreathing(CancellationToken cancellationToken)
        {
            breathingView.TotalTimeVisualizer.Run( breathingSettings.GetTotalTime(), cancellationToken);

            running = true;
            updateManager.RegisterUpdate(OnUpdate);
            breathingApi.StartSession(breathingSettings);
            
            for (int i = 0; i < breathingSettings.Rounds; i++)
            {
                audioManager.PlaySfx(settings.GetInhaleClip());
                await breathingView.BreathingVisualizer.Inhale(breathingSettings.GetInhaleDuration(), cancellationToken);
                if (breathingSettings.GetAfterInhaleDuration() > 0)
                {
                    audioManager.PlaySfx(settings.GetHoldClip());
                    await breathingView.BreathingVisualizer.InhaleWait(breathingSettings.GetAfterInhaleDuration(),
                        cancellationToken);
                }

                audioManager.PlaySfx(settings.GetExhaleClip());
                await breathingView.BreathingVisualizer.Exhale(breathingSettings.GetExhaleDuration(), cancellationToken);
                if (breathingSettings.GetAfterExhaleDuration() > 0)
                {
                    audioManager.PlaySfx(settings.GetHoldClip());
                    await breathingView.BreathingVisualizer.ExhaleWait(breathingSettings.GetAfterExhaleDuration(),
                        cancellationToken);
                }

                breathingView.BreathStatisticVisualizer.SetActual(i+1);
                breathingApi.IncreaseBreathingCountInSession();
            }
            updateManager.UnregisterUpdate(OnUpdate);

            running = false;
            breathingFinished = true;
            return true;
        }

        private void OnBack()
        {
            cancellationTokenSource.Cancel();
            breathingApi.FinishSession(breathingTime);
            updateManager.UnregisterUpdate(OnUpdate);
            uiManager.HideInfoPopup();
            StateMachine.SetStateAsync<MenuState>(
                StateData.Create((StateDataKeys.BreathingFinished,true)),
                false).Forget();
        }

        private void OnSettings()
        {
            ServiceLocator.Get<IUiManager>().ShowSettingsPopup(true);
            SetPaused(true);
        }

        private void OnPause() => SetPaused(!paused);

        private void SetPaused(bool paused)
        {
            this.paused = paused;
            if (paused)
            {
                breathingView.PauseText.Show().Forget();
            }
            else
            {
                breathingView.PauseText.Hide().Forget();
            }
            breathingView.BreathingVisualizer.IsPaused = paused;
            breathingView.TotalTimeVisualizer.IsPaused = paused;
        }

        private void OnUpdate(float dt)
        {
            if (running && !paused)
            {
                breathingTime += dt;
            }
        }
    }
}