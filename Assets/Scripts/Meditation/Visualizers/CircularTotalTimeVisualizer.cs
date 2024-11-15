using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Visualizers
{
    public class CircularTotalTimeVisualizer : ATotalTimeVisualizer
    {
        [SerializeField] private Image image;
 
        public override bool IsPaused { get; set; }

        
        public override void Initialize()
        {
            image.fillAmount = 1;
        }
        
        public override async UniTask Run(float totalTime, CancellationToken cancellationToken)
        {
            float timeLeft = totalTime;
         
            while (timeLeft > 0)
            {
                float normalizedTime = 1 - Mathf.Clamp01((totalTime - timeLeft) / totalTime);
                image.fillAmount = normalizedTime;
                await UniTask.Yield(cancellationToken);
                if (!IsPaused)
                {
                    timeLeft -= Time.deltaTime;
                }
                if (cancellationToken.IsCancellationRequested)
                    return;
            }

            image.fillAmount = 1;
        }
    }
}