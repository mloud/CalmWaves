using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;

namespace Meditation.Managers.Messages.Ui
{
    public class MessageComponent : MonoBehaviour
    {
        [SerializeField] private float initialDelay;
        [SerializeField] private float visibleDuration;
        [SerializeField] private float hidenDuration;
        [SerializeField] private string area;
        [SerializeField] private AExtendedText extendedText;
       
        private CancellationTokenSource cancellationTokenSource;

        private void Awake() => extendedText.text = "";
        public void StartDisplaying() => StartDisplayingAsync().Forget();
        public void StopDisplaying() => cancellationTokenSource?.Cancel();

        private async UniTask StartDisplayingAsync()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            var messageManager = ServiceLocator.Get<IMessageManager>();

            try
            {
                await UniTask.WaitForSeconds(initialDelay, false, PlayerLoopTiming.Update,
                    cancellationTokenSource.Token);
                extendedText.enabled = true;
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var message = messageManager.GetNextMessage(area);
                    if (message != null)
                    {
                        extendedText.Set(message);
                        await UniTask.WaitForSeconds(visibleDuration, false, PlayerLoopTiming.Update,
                            cancellationTokenSource.Token);
                        extendedText.Set("");
                        await UniTask.WaitForSeconds(hidenDuration, false, PlayerLoopTiming.Update,
                            cancellationTokenSource.Token);
                        
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {

            }
            finally
            {
                extendedText.Set("");
            }
        }
    }
}