using System;
using DG.Tweening;
using UnityEngine;

namespace Meditation.Tweens
{
    public class PulsateTween : MonoBehaviour
    {
        public float SpeedMultiplier = 1.0f;
        [SerializeField] private Vector3 endValue;
        [SerializeField] private float duration;
        [SerializeField] private float delay;

        private void OnValidate()
        {
            duration = 0.04f;
        }

        private void OnEnable()
        {
            transform.DOScale(endValue, duration).SetLoops(-1, LoopType.Yoyo) // Infinite loop with ping-pong effect
                .SetEase(Ease.InOutSine).From(Vector3.one).SetDelay(delay);
        }

        private void OnDisable()
        {
            DOTween.KillAll(transform);
        }

        void Update()
        {
            float scaleX = Mathf.PingPong(Time.time * duration * SpeedMultiplier, endValue.x - 1);
            float scaleY = Mathf.PingPong(Time.time * duration * SpeedMultiplier, endValue.y - 1);

            transform.localScale = new Vector3(1 + scaleX, 1 + scaleY, 1);
        }
        
    }
}