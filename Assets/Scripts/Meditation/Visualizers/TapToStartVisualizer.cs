using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Meditation.Visualizers
{
    public class TapToStartVisualizer : ACountDownVisualizer
    {
        [SerializeField] private CanvasGroup cg;
        
        private bool isTouchPressed;

        private void Awake()
        {
            cg.alpha = 0;
        }
        
        public override async UniTask Run(CancellationToken cancellationToken, Action onHideStart)
        {
            isTouchPressed = false;
            await cg.DOFade(1, 0.5f).From(0).SetEase(Ease.Linear).AsyncWaitForCompletion();
            await UniTask.WaitUntil(() => isTouchPressed, cancellationToken: cancellationToken);
            onHideStart?.Invoke();
            await cg.DOFade(0, 0.5f).SetEase(Ease.Linear).AsyncWaitForCompletion();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isTouchPressed = true;
            }
        }
    }
}