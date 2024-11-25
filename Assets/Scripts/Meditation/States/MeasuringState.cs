using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Data;
using Meditation.Apis.Measure;
using Meditation.Ui;
using Meditation.Ui.Panels;
using Meditation.Ui.Views;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Meditation.States
{
    public class MeasuringState : AState
    {
        private bool isFingerOnTheScreen;
        private CancellationTokenSource ctx;
        private MeasuringView view;
        private BreathingTestResult result;

        private enum MeasurementType
        {
            Exhale,
            Inhale
        }
        
        public override UniTask Initialize()
        {
            view = ServiceLocator.Get<IUiManager>().GetView<MeasuringView>();
            view.MouseHandler.onPointerDown.AddListener(OnFingerDown);
            view.MouseHandler.onPointerUp.AddListener(OnFingerUp);
            view.BindAction(view.BackButton, OnBack);
            view.BindAction(view.SaveButton, OnSave);
            view.BindAction(view.MeasuringButton, History);
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ctx?.Dispose();
            ctx = new CancellationTokenSource();
            Debug.Log("[State]  EnterAsync started");
            isFingerOnTheScreen = false;
            view.TitleLabel.text = "Track Your Breathing";
            view.SubtitleLabel.text = "Measure Inhale and Exhale Durations for Better Awareness";

            await view.SubtitleLabel.SetVisibleWithFade(false, 0, false);
            await view.Result.SetVisibleWithFade(false, 0, true);
            await view.TapToStartCircle.SetVisibleWithFade(false, 0, true);
            await view.SaveButton.SetVisibleWithFade(false, 0, true);
            await view.MeasureCircle.SetVisibleWithFade(false, 0, true);
            await view.Prompt.SetVisibleWithFade(false, 0, false);

            ServiceLocator.Get<IUiManager>().GetPanel<TopHudPanel>().Hide(true).Forget();
            
            await view.Show(true);
            Debug.Log("[State]  EnterAsync finished");
        }

        public override async UniTask ExecuteAsync()
        {
            Debug.Log("[State]  ExecuteAsync started");
            var measurementResults = new Dictionary<MeasurementType, TimeSpan>();
        
            await view.SubtitleLabel.SetVisibleWithFade(true, 1.0f, false);
            await UniTask.WaitForSeconds(1);
            await view.SubtitleLabel.SetVisibleWithFade(false, 1.0f, false);
            bool measuringCancelled = false;

            try
            {
                // EXHALE
                bool exhaleFinished;
                do
                {
                    var (duration, measurementFinished) =
                        await StartMeasuring(
                            true,
                            MeasurementTexts.ForExhale());
                    exhaleFinished = measurementFinished;
                    if (measurementFinished)
                    {
                        measurementResults.Add(MeasurementType.Exhale, duration);
                    }

                } while (!exhaleFinished);

                view.TitleLabel.Set( "");
                await view.TapToStartCircle.SetVisibleWithFade(false, 0.5f, true);
                view.Prompt.Set("That's a great result, take a break");
                await UniTask.WaitForSeconds(4.0f);
                view.Prompt.Set("Tap the screen to continue when you are ready");
                await UniTask.WaitUntil(()=>Input.GetMouseButtonDown(0), cancellationToken:ctx.Token, cancelImmediately:true);
                await view.TapToStartCircle.SetVisibleWithFade(true, 0.5f, true);

                
                // INHALE
                bool inhaleFinished;
                do
                {
                    var (duration, measurementFinished) =
                        await StartMeasuring(
                            false,
                            MeasurementTexts.ForInhale());
                    inhaleFinished = measurementFinished;
                    if (measurementFinished)
                    {
                        measurementResults.Add(MeasurementType.Inhale, duration);
                    }
                    
                    view.TitleLabel.Set("");
                    view.Prompt.Set("That's a great result");
                    await UniTask.WaitForSeconds(3.0f);

                } while (!inhaleFinished);
            }
            catch (OperationCanceledException _)
            {
                measuringCancelled = true;
                Debug.Log("Measuring cancelled");
            }
        
            if (!measuringCancelled)
            {
                view.InhaleValue.text = $"{Math.Round(measurementResults[MeasurementType.Inhale].TotalSeconds)} sec";
                view.ExhaleValue.text = $"{Math.Round(measurementResults[MeasurementType.Exhale].TotalSeconds)} sec";


                result = new BreathingTestResult(
                    DateTime.Now,
                    ("Exhale", measurementResults[MeasurementType.Exhale]),
                    ("Inhale", measurementResults[MeasurementType.Inhale]));
                
                await UniTask.WhenAll(
                    view.TimerLabel.SetVisibleWithFade(false, 1.0f, true),
                    view.TapToStartCircle.SetVisibleWithFade(false, 1.0f, true),
                    view.MeasureCircle.SetVisibleWithFade(false, 1.0f, true));

                await UniTask.WhenAll(
                    view.Result.SetVisibleWithFade(true, 1.0f, true),
                    view.SaveButton.SetVisibleWithFade(true, 1.0f, true));
            }
            Debug.Log("[State]  ExecuteAsync finished");
        }


        public class MeasurementTexts
        {
            public static MeasurementTexts ForExhale() => new MeasurementTexts
            {
                Title = "Exhale",
                BeforeSentence = "Take a deep breath, then tap and hold the screen to exhale as long as you can",
                DuringSentence = "Keep exhaling, release when you’re done",
            };
            
            public static MeasurementTexts ForInhale() => new MeasurementTexts
            {
                Title = "Inhale",
                BeforeSentence = "Exhale fully, then tap and hold the screen to inhale as deeply as you can",
                DuringSentence  = "Keep inhaling, release when you’re done",
            };
            
            public string Title { get; private set; }
            public string BeforeSentence { get; private set; }
            public string DuringSentence { get; private set;  }
        }

        private async UniTask<(TimeSpan duration, bool wasFinished)> StartMeasuring(bool animateInside, 
          MeasurementTexts measurementTexts)
        {
            view.Prompt.Set(measurementTexts.BeforeSentence);
            view.TimerLabel.text = "";

            await UniTask.WhenAll(
                view.TapToStartCircle.SetVisibleWithFade(true, 2.0f, true),
                view.Prompt.SetVisibleWithFade(true, 2.0f, false));

            await UniTask.WaitUntil(() => isFingerOnTheScreen, cancellationToken: ctx.Token, cancelImmediately:true);
            view.MeasureCircle.SetVisibleWithFade(true, 0.5f, true).Forget();
            view.TitleLabel.SetVisibleWithFade(true, 0.5f, false).Forget();
           
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            view.TitleLabel.Set(measurementTexts.Title);
            view.Prompt.Set(measurementTexts.DuringSentence);
            const float maxTime = 20;
            while (isFingerOnTheScreen)
            {
                view.TimerLabel.text = stopWatch.Elapsed.TotalSeconds
                    .ToString("F1")+"\n<size=90>sec</size>";
                var progress = Mathf.Clamp01((float)stopWatch.Elapsed.TotalSeconds / maxTime);
                if (animateInside)
                    progress = 1 - progress;
                
                view.MeasureCircle.transform.localScale = new Vector3(progress, progress, 1.0f);
                await UniTask.Yield(cancellationToken:ctx.Token);
            }

            stopWatch.Stop();
            view.MeasureCircle.SetVisibleWithFade(false, 0.5f, true).Forget();
            return (stopWatch.Elapsed, true);
        }
        
        public override async UniTask ExitAsync()
        {
            ctx.Cancel();
            ServiceLocator.Get<IUiManager>().GetPanel<TopHudPanel>().Show(true).Forget();   
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
            Debug.Assert(result != null);
            Debug.Log("OnSave");
            ServiceLocator.Get<IMeasureApi>().SaveBreathingTestResult(result);
            result = null;
            view.SaveButton.SetVisibleWithFade(false, 0.3f, true).Forget();
        }

        private async UniTask History()
        {
            var request = ServiceLocator.Get<IUiManager>().OpenPopup<MeasureHistoryPopup>(null);
            request.Popup.BindAction(request.Popup.CloseButton, () => request.Popup.Close(), true);
            await request.OpenTask;
        }
    }
}