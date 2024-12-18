using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Audio;
using Meditation.Ui.Audio;
using Meditation.Ui.Views;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace Meditation.States
{
    public class SleepState : AState
    {
        private SleepView view;
        private IUiManager uiManager;
        private IAudioEnvironmentManager audioEnvironmentManager;
        private CancellationTokenSource cancellationTokenSource;
        public override async UniTask Initialize()
        {
            audioEnvironmentManager = ServiceLocator.Get<IAudioEnvironmentManager>();
            uiManager = ServiceLocator.Get<IUiManager>();
            view = uiManager.GetView<SleepView>();
            view
                .BindAction(view.BackButton, OnBackButtonClicked)
                .BindAction(view.AudioButton, OnAudioClicked)
                .BindAction(view.ContinueButton, OnContinueClicked);
            
        
            view.TimerContainer.SetVisibleWithFade(false, 0, true);
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            view.HoursValueChanger.Initialize(0, 0,59);
            view.MinutesValueChanger.Initialize(10, 0,59);
            view.SecondsValueChanger.Initialize(0, 0,59);
            
            view.SettingsContainer.SetVisibleWithFade(false, 0.0f, true);
            view.RunningContainer.SetVisibleWithFade(false, 0.0f, true);
            view.FinishedContainer.SetVisibleWithFade(false, 0.0f, true);
            
            audioEnvironmentManager.Apply("sleepDefault");
            await view.Show(true);
            view.TimerContainer.SetVisibleWithFade(true, 1.0f, true);
            await view.SettingsContainer.SetVisibleWithFade(true, 1.0f, true);
        }

        public override async UniTask ExecuteAsync()
        { }

        public override async UniTask ExitAsync()
        {
            cancellationTokenSource?.Cancel();
            audioEnvironmentManager.Save(audioEnvironmentManager.Settings);
            audioEnvironmentManager.Apply("default");
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
           
            await view.Hide(true);
        }
      
        private void OnBackButtonClicked() => 
            StateMachine.SetStateAsync<MenuState>(waitForCurrentStateExit:false).Forget();
        
        private async UniTask OnAudioClicked()
        {
            var request = uiManager.OpenPopup<AudioPopup>(null);
            await request.OpenTask;
        }

        private async UniTask OnContinueClicked()
        {
            var ts = new TimeSpan();
            ts += TimeSpan.FromHours(view.HoursValueChanger.Value);
            ts += TimeSpan.FromMinutes(view.MinutesValueChanger.Value);
            ts += TimeSpan.FromSeconds(view.SecondsValueChanger.Value);

            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
         
            view.HoursValueChanger.Set(ts.Hours);
            view.MinutesValueChanger.Set(ts.Minutes);
            view.SecondsValueChanger.Set(ts.Seconds);
            
            view.HoursValueChanger.SetButtonsVisible(false, true);
            view.MinutesValueChanger.SetButtonsVisible(false, true);
            view.SecondsValueChanger.SetButtonsVisible(false, true);

            
            await view.SettingsContainer.SetVisibleWithFade(false, 0.5f, true);
            await view.RunningContainer.SetVisibleWithFade(true, 0.5f, true);
            StartTimer(ts, cancellationTokenSource.Token);
        }


        private async UniTask StartTimer(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            while (timeSpan.TotalSeconds > 0)
            {
                timeSpan -= TimeSpan.FromSeconds(Time.deltaTime);
                view.HoursValueChanger.Set(timeSpan.Hours);
                view.MinutesValueChanger.Set(timeSpan.Minutes);
                view.SecondsValueChanger.Set(timeSpan.Seconds);
                
                await UniTask.Yield(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            audioEnvironmentManager.StopAll(5);
            
            await view.TimerContainer.SetVisibleWithFade(false, 1.0f, true);
            await view.RunningContainer.SetVisibleWithFade(false, 1.0f, true);
            await view.FinishedContainer.SetVisibleWithFade(true, 1.0f, true);
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}