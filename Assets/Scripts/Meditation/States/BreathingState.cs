using System.Threading;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Settings;
using Meditation.Ui;
using Meditation.Ui.Views;
using UnityEngine;

namespace Meditation.States
{
    public class BreathingState : AState
    {
        private BreathingView breathingView;
        private CancellationTokenSource cancellationTokenSource;
        private ISoundSettingsModule settings;

        private AddressableAsset<AudioClip> winClip;

        private bool breathingFinished;
        public override async UniTask Initialize()
        {
            settings = ServiceLocator.Get<ISettingsApi>().GetModule<ISoundSettingsModule>();
            breathingView = LookUp.Get<BreathingView>().GetFirst();
            breathingView
                .BindAction(breathingView.PauseButton, OnPause)
                .BindAction(breathingView.BackButton, OnBack)
                .BindAction(breathingView.SettingsButton, OnSettings);
           
            winClip = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<AudioClip>("win");
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            breathingFinished = false;
            Debug.Assert(stateData != null, "State data is required when entering BreathingState");
            var settings = stateData.GetValue<IBreathingSettings>("Settings");
         
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            breathingView.TotalTimeVisualizer.Initialize();
            breathingView.BreathingVisualizer.Initialize();
            breathingView.TextFader.Clear();
            breathingView.PauseText.Clear();
            breathingView.NameLabel.text = settings.GetName();
            breathingView.BreathStatisticVisualizer.Init(settings.Rounds());
            
            await breathingView.Show(false);
            await breathingView.HideElements();
            await StartBreathing(settings, cancellationTokenSource.Token);
        }

        public override UniTask ExecuteAsync()
        {
            return UniTask.CompletedTask;
        }

        public override async UniTask ExitAsync()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            ServiceLocator.Get<IAudioManager>().StopMusic();
            await breathingView.Hide(true);
        }
        
        private async UniTask<bool> StartBreathing(IBreathingSettings breathingSettings, CancellationToken cancellationToken)
        {
            var audioManager = ServiceLocator.Get<IAudioManager>();
            ServiceLocator.Get<IUiManager>().ShowInfoPopup(breathingSettings, false, false);
            await breathingView.CountDownVisualizer.Run( 
                cancellationToken, 
                ()=> ServiceLocator.Get<IUiManager>().HideInfoPopup());
          
            if (cancellationToken.IsCancellationRequested) return false;
            await breathingView.FadeInElements();
            audioManager.PlayMusic(breathingSettings.GetMusic()).Forget();
           
            breathingView.TotalTimeVisualizer.Run( breathingSettings.GetTotalTime(), cancellationToken);
            for (int i = 0; i < breathingSettings.Rounds(); i++)
            {
                audioManager.PlaySfx(settings.GetInhaleClip());
                await breathingView.BreathingVisualizer.Inhale(breathingSettings.GetInhaleDuration(), cancellationToken);
                if (cancellationToken.IsCancellationRequested) return false;
                if (breathingSettings.GetAfterInhaleDuration() > 0)
                {
                    audioManager.PlaySfx(settings.GetHoldClip());
                    await breathingView.BreathingVisualizer.InhaleWait(breathingSettings.GetAfterInhaleDuration(),
                        cancellationToken);
                }

                audioManager.PlaySfx(settings.GetExhaleClip());
                if (cancellationToken.IsCancellationRequested) return false;  
                await breathingView.BreathingVisualizer.Exhale(breathingSettings.GetExhaleDuration(), cancellationToken);
                if (cancellationToken.IsCancellationRequested) return false;
                if (breathingSettings.GetAfterExhaleDuration() > 0)
                {
                    audioManager.PlaySfx(settings.GetHoldClip());
                    await breathingView.BreathingVisualizer.ExhaleWait(breathingSettings.GetAfterExhaleDuration(),
                        cancellationToken);
                }

                breathingView.BreathStatisticVisualizer.SetActual(i+1);
            }

            await ServiceLocator.Get<IBreathingApi>().RegisterFinishedBreathing(breathingSettings);
            audioManager.PlaySfx(winClip.GetReference());
            await breathingView.FadeOutElements();
            await breathingView.TextFader.Show();

            breathingFinished = true;
            
            return true;
        }

        private void OnBack()
        {
            ServiceLocator.Get<IUiManager>().HideInfoPopup();
            StateMachine.SetStateAsync<MenuState>(
                StateData.Create((StateDataKeys.BreathingFinished,true)),
                false).Forget();
        }

        private void OnSettings()
        {
            ServiceLocator.Get<IUiManager>().ShowSettingsPopup(true);
            SetPaused(true);
        }

        private void OnPause()
        {
            bool isPaused = breathingView.BreathingVisualizer.IsPaused;
            SetPaused(!isPaused);
        }

        private void SetPaused(bool paused)
        {
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
    }
}