using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Data;
using Meditation.Apis.Settings;
using Meditation.Ui;
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
                .BindAction(breathingView.SettingsButton, OnSettingsClicked);
           
            winClip = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<AudioClip>("win");
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            breathingTime = 0;
            paused = false;
            breathingFinished = false;
            Debug.Assert(stateData != null, "State data is required when entering BreathingState");
            breathingSettings = stateData.GetValue<IBreathingSettings>("Settings");
         
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            // initialize
            breathingView.TotalTimeVisualizer.Initialize();
            breathingView.BreathingVisualizer.Initialize();
            breathingView.PauseText.Clear();
            breathingView.NameLabel.text = breathingSettings.GetName();
            breathingView.BreathStatisticVisualizer.Init(breathingSettings.Rounds);

            try
            {
                bool canceled = false;
                var request = uiManager.OpenPopup<InfoPopup>(UiParameter.Create(breathingSettings));
                request.Popup.CloseButton.gameObject.SetActive(true);
                request.Popup.ContinueButton.gameObject.SetActive(true);
                request.Popup.BindAction(request.Popup.CloseButton,
                    () =>
                    {
                        canceled = true;
                        StateMachine.SetStateAsync<MenuState>().Forget();
                        request.Popup.Close().Forget();
                    });
                audioManager.PlayMusic(breathingSettings.GetMusic()).Forget();
                request.Popup.BindAction(request.Popup.ContinueButton, () => request.Popup.Close().Forget());

                await request.OpenTask;
                await request.WaitForClose();
                if (canceled) return;

                await breathingView.Show(true);
                await breathingView.FadeInElements();
             

                // breathing
                await CountBreathing(cancellationTokenSource.Token);
                audioManager.PlaySfx(winClip.GetReference());

                FinishedBreathing finishedBreathing = null;
                if (!cancellationTokenSource.IsCancellationRequested)
                    finishedBreathing = await breathingApi.FinishSession(breathingSettings.GetTotalTime());

                if (!cancellationTokenSource.IsCancellationRequested)
                    await breathingView.FadeOutElements(true);

                var popupRequest = ServiceLocator.Get<IUiManager>()
                    .OpenPopup<BreathingFinishedPopup>(UiParameter.Create(finishedBreathing));

                await popupRequest.OpenTask;
                await popupRequest.WaitForClose();

                StateMachine.SetStateAsync<MenuState>(
                    StateData.Create((StateDataKeys.BreathingFinished, true)),
                    false).Forget();
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Breathing was cancelled");
            }
            finally
            {
                SetPaused(false, false);
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

        private async UniTask OnBack()
        {
            cancellationTokenSource.Cancel();
            var finishedBreathing = await breathingApi.FinishSession(breathingTime);
            updateManager.UnregisterUpdate(OnUpdate);
            
            await breathingView.FadeOutElements(true);

            if (finishedBreathing != null && finishedBreathing.Breaths > 0)
            {
                var popupRequest = ServiceLocator.Get<IUiManager>()
                    .OpenPopup<BreathingFinishedPopup>(UiParameter.Create(finishedBreathing));

                await popupRequest.OpenTask;
                await popupRequest.WaitForClose();
            }

            StateMachine.SetStateAsync<MenuState>(
                StateData.Create((StateDataKeys.BreathingFinished,true)),
                false).Forget();
        }

        private async UniTask OnSettingsClicked()
        {
            var request = ServiceLocator.Get<IUiManager>().OpenPopup<SettingsPopup>(null);
            request.Popup.BindAction(request.Popup.CloseButton, () => request.Popup.Close(), true);
            request.OpenTask.Forget();
            SetPaused(true, false);
            await request.WaitForClose();
            SetPaused(false, false);
        }

        private void OnPause() => SetPaused(!paused,true);

        private void SetPaused(bool paused, bool showLabel)
        {
            this.paused = paused;
            if (showLabel)
            {
                if (paused)
                {
                    breathingView.PauseText.Show().Forget();
                }
                else
                {
                    breathingView.PauseText.Hide().Forget();
                }
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