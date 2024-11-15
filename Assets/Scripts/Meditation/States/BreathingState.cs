using System.Threading;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Ui;
using Meditation.Ui.Views;
using UnityEngine;

namespace Meditation.States
{
    public class BreathingState : AState
    {
        private BreathingView breathingView;
        private CancellationTokenSource cancellationTokenSource;

        private AddressableAsset<AudioClip> audioClip;
        private AddressableAsset<AudioClip> winClip;

        private bool breathingFinished;
        public override async UniTask Initialize()
        {
            breathingView = LookUp.Get<BreathingView>().GetFirst();
            breathingView
                .BindAction(breathingView.PauseButton, OnPause)
                .BindAction(breathingView.BackButton, OnBack);
           
            audioClip = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<AudioClip>("beep");
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
                await breathingView.BreathingVisualizer.Inhale(breathingSettings.GetInhaleDuration(), cancellationToken);

                audioManager.PlaySfx(audioClip.GetReference());
                if (cancellationToken.IsCancellationRequested) return false;    
                await breathingView.BreathingVisualizer.InhaleWait(breathingSettings.GetAfterInhaleDuration(), cancellationToken);
                audioManager.PlaySfx(audioClip.GetReference());
                if (cancellationToken.IsCancellationRequested) return false;  
                await breathingView.BreathingVisualizer.Exhale(breathingSettings.GetExhaleDuration(), cancellationToken);
                if (cancellationToken.IsCancellationRequested) return false;
                audioManager.PlaySfx(audioClip.GetReference());
                await breathingView.BreathingVisualizer.ExhaleWait(breathingSettings.GetAfterExhaleDuration(), cancellationToken);
                audioManager.PlaySfx(audioClip.GetReference());

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
            StateMachine.SetStateAsync<MenuState>(
                StateData.Create((StateDataKeys.BreathingFinished,true)),
                false).Forget();
        }
        
        private void OnPause()
        {
            if (breathingView.BreathingVisualizer.IsPaused)
            {
                breathingView.PauseText.Hide().Forget();
                breathingView.BreathingVisualizer.IsPaused = false;
                breathingView.TotalTimeVisualizer.IsPaused = false;
            }
            else
            {
                breathingView.PauseText.Show().Forget();
                breathingView.BreathingVisualizer.IsPaused = true;
                breathingView.TotalTimeVisualizer.IsPaused = true;
            }
        }
    }
}