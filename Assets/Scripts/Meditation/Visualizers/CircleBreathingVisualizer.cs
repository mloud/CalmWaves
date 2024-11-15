using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Meditation.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Visualizers
{
    public class CircleBreathingVisualizer : ABreathingVisualizer
    {
        [SerializeField] private Image circleImage;
        [SerializeField] private SmoothText label;
        [SerializeField] private Color exhaleColor;
        [SerializeField] private Color inhaleColor;
        
        public override bool IsPaused { get; set; }
        
        public override void Initialize()
        {
            label.Set("");
            circleImage.transform.localScale = new Vector3(0, 0, 1);
        }

        public override async UniTask Inhale(float duration, CancellationToken cancellationToken)
        {
            label.Set("Inhale");
            circleImage.color = inhaleColor;
            await CountNormalizedTime(duration,
                nTime=> circleImage.transform.localScale = new Vector3(nTime, nTime, 1.0f),
                cancellationToken);
        }

        public override async UniTask InhaleWait(float duration, CancellationToken cancellationToken)
        {
            if (duration <= 0)
                return; 
            label.Set("Hold");
            circleImage.color = inhaleColor;
            await CountNormalizedTime(duration, null, cancellationToken);
        }

        public override async UniTask Exhale(float duration, CancellationToken cancellationToken)
        {
            label.Set("Exhale");
            circleImage.color = exhaleColor;
            await CountNormalizedTime(duration, 
                nTime => circleImage.transform.localScale = new Vector3(1-nTime, 1-nTime, 1.0f),
                cancellationToken);
        }

        public override async UniTask ExhaleWait(float duration, CancellationToken cancellationToken)
        {
            if (duration <= 0)
                return;
            
            label.Set("Hold");
            circleImage.color = exhaleColor;
            await CountNormalizedTime(duration, null, cancellationToken);
        }

        private async UniTask CountNormalizedTime(float duration, Action<float> action, CancellationToken cancellationToken)
        {
            float timeLeft = duration;
            while (timeLeft > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                float normalizedTime = Mathf.Clamp01((duration - timeLeft) / duration);
                action?.Invoke(normalizedTime);
                if (!IsPaused)
                {
                    timeLeft -= Time.deltaTime;
                }
                await UniTask.Yield(cancellationToken);
             
            }
            action?.Invoke(1);
        }
    }
}