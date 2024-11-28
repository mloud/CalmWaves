using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Ui.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class GenerateButton : MonoBehaviour
    {
        public Button Button => button;

        private CancellationTokenSource cancellationTokenSource;
        
        [SerializeField] private GameObject shine;
        [SerializeField] private PulsateTween pulsateTween;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;
        public void Reset()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            label.text = "Start";
            pulsateTween.SpeedMultiplier = 1.0f;
            SetButtonActive(false).Forget();
            button.image.raycastTarget = true;
        }

        public async UniTask AnimateToGenerating()
        {
            label.text = "Generating";
            pulsateTween.SpeedMultiplier = 5.0f;
            button.image.raycastTarget = false;
        }

        public async UniTask SetButtonActive(bool active)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();

            cancellationTokenSource = new CancellationTokenSource();
            button.interactable = active;
            try
            {
                await shine.SetVisibleWithFade(active, 0.2f, false, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Fade operation was canceled.");
            }
        }
    }
}