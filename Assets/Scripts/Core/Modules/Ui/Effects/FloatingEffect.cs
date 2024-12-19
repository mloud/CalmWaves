using System;
using DG.Tweening;
using UnityEngine;

namespace Core.Modules.Ui.Effects
{
    public class FloatingEffect : BaseUiEffect
    {
        [SerializeField] private float height;
        [SerializeField] private float duration = 1.0f;
        [SerializeField] private float delay;
        [SerializeField] private Ease ease;
        private Tween tween;
       
        public override void Run()
        {
            tween?.Kill();
            tween = transform.DOLocalMoveY(transform.localPosition.y + height, duration)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(delay);
        }

        public override void Stop()
        {
            tween?.Kill();
            tween = null;
        }

        public override bool IsPlaying() => tween != null;
    }
}