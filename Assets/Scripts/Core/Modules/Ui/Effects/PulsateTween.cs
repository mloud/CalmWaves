using System;
using DG.Tweening;
using UnityEngine;

namespace OneDay.Core.Modules.Ui.Effects
{
    public class PulsateTween : MonoBehaviour
    {
        public float SpeedMultiplier = 1.0f;
      
        [SerializeField] private Vector3 endValue;
        [SerializeField] private float duration;
        [SerializeField] private float delay;

        private Tween tween;
        private void OnEnable()
        {
            tween = transform.DOScale(endValue, duration).SetLoops(-1, LoopType.Yoyo) // Infinite loop with ping-pong effect
                .SetEase(Ease.InOutSine).From(Vector3.one).SetDelay(delay);
        }

        private void OnDisable()
        {
            tween?.Kill(true);
            tween = null;
        }

        void Update()
        {
            float scaleX = Mathf.PingPong(Time.time * duration * SpeedMultiplier, endValue.x - 1);
            float scaleY = Mathf.PingPong(Time.time * duration * SpeedMultiplier, endValue.y - 1);

            transform.localScale = new Vector3(1 + scaleX, 1 + scaleY, 1);
        }
        
    }
}