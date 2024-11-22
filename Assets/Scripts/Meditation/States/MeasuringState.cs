using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Meditation.Ui.Views;
using Debug = UnityEngine.Debug;

namespace Meditation.States
{
    public class MeasuringState : AState
    {
        private MeasuringView view;
        private bool isFingerOnTheScreen;

        private CancellationTokenSource ctx;
        
        public override UniTask Initialize()
        {
            view = ServiceLocator.Get<IUiManager>().GetView<MeasuringView>();
            view.MouseHandler.onPointerDown.AddListener(OnFingerDown);
            view.MouseHandler.onPointerUp.AddListener(OnFingerUp);

            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ctx?.Dispose();
            ctx = new CancellationTokenSource();
            Debug.Log("[State]  EnterAsync started");
            isFingerOnTheScreen = false;
            view.BindAction(view.BackButton, OnBack);
            view.BindAction(view.SaveButton, OnSave);
            await view.Result.SetVisibleWithFade(false, 0, true);
            await view.TapToStartCircle.SetVisibleWithFade(true, 0, true);
            await view.SaveButton.SetVisibleWithFade(false, 0, true);
            await view.MeasureCircle.SetVisibleWithFade(false, 0, true);
            await view.Prompt.SetVisibleWithFade(false, 0, false);
            await view.Show(true);
            Debug.Log("[State]  EnterAsync finished");
        }

        public override async UniTask ExecuteAsync()
        {
            Debug.Log("[State]  ExecuteAsync started");
            var measurementNames = new[] { "Inhale", "Exhale" };
            var measurementResults = new List<TimeSpan>();
            var measurementIndex = 0;

            while (measurementIndex < 2)
            {
                var duration = await 
                    StartMeasuring(measurementNames[measurementIndex], 3, measurementIndex == 1);
                if (duration != null)
                {
                    measurementResults.Add(duration.Value);
                    measurementIndex++;
                }
            }

            view.InhaleValue.text = $"{Math.Round(measurementResults[0].TotalSeconds)} sec";
            view.ExhaleValue.text = $"{Math.Round(measurementResults[1].TotalSeconds)} sec";

            
            await UniTask.WhenAll(
                view.TapToStartCircle.SetVisibleWithFade(false, 1.0f, true),
                view.MeasureCircle.SetVisibleWithFade(false, 1.0f, true));

            await UniTask.WhenAll(
                view.Result.SetVisibleWithFade(true, 1.0f, true),
                view.SaveButton.SetVisibleWithFade(true, 1.0f, true));
            
            Debug.Log("[State]  ExecuteAsync finished");
        }


        private async UniTask<TimeSpan?> StartMeasuring(string what, int countDown, bool isLast)
        {
            view.Prompt.text = $"Touch the screen to measure {what}";
            await view.Prompt.SetVisibleWithFade(true, 0.3f, false);

            //try
           // {
                await UniTask.WaitUntil(() => isFingerOnTheScreen, cancellationToken: ctx.Token, cancelImmediately:true);
           // }
           // catch (OperationCanceledException ex)
           // {
           //     Debug.Log("Cancelled");
           // }

            // COUNTING DOWN
            view.TimerLabel.text = "";
            view.Prompt.text = $"Prepare for a long {what}";
            await view.MeasureCircle.SetVisibleWithFade(true, 0.5f,true);
           
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            bool isMeasuringInterrupted = false;
            while (stopWatch.Elapsed.TotalSeconds <= countDown)
            {
                // wait
                view.TimerLabel.text = 
                    ((int)(countDown - stopWatch.Elapsed.TotalSeconds))
                    .ToString(CultureInfo.InvariantCulture);
                // finger was released before measuring started
                if (!isFingerOnTheScreen)
                {
                    isMeasuringInterrupted = true;
                    break;
                }

                await UniTask.Yield(cancellationToken:ctx.Token);
            }

            if (isMeasuringInterrupted)
                return null;
                
            stopWatch.Restart();
            view.Prompt.text = $"{what}";
            while (isFingerOnTheScreen)
            {
                view.TimerLabel.text = stopWatch.Elapsed.TotalSeconds
                    .ToString("F1");
                await UniTask.Yield(cancellationToken:ctx.Token);
            }

            stopWatch.Stop();


            if (isLast)
            {
                view.Prompt.text = "That's a great result ";
            }
            else
            {
                view.Prompt.text = "Great result. Take your breath and we will continue";
            }
            return stopWatch.Elapsed;
        }
        
        public override async UniTask ExitAsync()
        {
            ctx.Cancel();
               
            await view.Hide(true);
            Debug.Log("[State] ExitAsync finished");
        }

        private void OnBack()
        {
            Debug.Log("[State] OnBack");

            StateMachine.SetStateAsync<MenuState>(
                null,
                false).Forget();
        }

        private void OnFingerUp() => isFingerOnTheScreen = false;

        private void OnFingerDown() => isFingerOnTheScreen = true;
        
        private void OnSave()
        {
            Debug.Log("OnSave");
            view.SaveButton.SetVisibleWithFade(false, 0.3f, true).Forget();
        }
    }
}