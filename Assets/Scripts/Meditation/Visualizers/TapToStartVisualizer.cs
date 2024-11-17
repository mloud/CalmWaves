using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Visualizers
{
    public class TapToStartVisualizer : ACountDownVisualizer
    {
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private Button continueButton;
        private bool isTouchPressed;

        private void Awake()
        {
            cg.alpha = 0;
            continueButton.onClick.AddListener(OnContinue);
        }

        private void OnContinue() => 
            isTouchPressed = true;

        public override async UniTask Run(CancellationToken cancellationToken, Action onHideStart)
        {
            gameObject.SetActive(true);
            isTouchPressed = false;
            await cg.DOFade(1, 0.5f).From(0).SetEase(Ease.Linear).AsyncWaitForCompletion();
            await UniTask.WaitUntil(() => isTouchPressed, cancellationToken: cancellationToken);
            onHideStart?.Invoke();
            await cg.DOFade(0, 0.5f).SetEase(Ease.Linear).AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }
    }
}