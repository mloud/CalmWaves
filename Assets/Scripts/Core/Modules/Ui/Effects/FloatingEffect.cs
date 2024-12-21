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

        private Vector3? originaPosition;
        
        public override void Run()
        {
            originaPosition ??= transform.localPosition;
            tween?.Kill();
            tween = transform.DOLocalMoveY(transform.localPosition.y + height, duration)
                .SetEase(ease)
                .From(originaPosition.Value.y)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(delay);
        }

        public override void Stop()
        {
            tween?.Kill();
            tween = null;
        }

        public override bool IsPlaying() => tween != null;

        private void OnDisable()
        {
            if (originaPosition != null)
            {
                transform.localPosition = originaPosition.Value;
            }

            Stop();
        }
    }
}