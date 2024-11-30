using System;
using DG.Tweening;
using UnityEngine;

namespace Core.Modules.Ui.Effects
{
    public class SpringMoveTween : MonoBehaviour
    {
        public Transform Target
        {
            get => target;
            set => target = value;
        }
        
        [SerializeField] private Transform target;
        [SerializeField] private float normalizedDistance = 1;
        [SerializeField] private float delay;
        [SerializeField] private float duration;
        [SerializeField] private bool startOnEnable;

        private Vector3 originaPosition;

        private Tween tween;
       
        public void Run()
        {
            originaPosition = transform.position;
            tween = transform.DOMove(transform.position + (target.position - transform.position) * normalizedDistance,
                    duration)
                .SetLoops(-1, LoopType.Yoyo) // Infinite loop with ping-pong effect
                .SetEase(Ease.InOutSine)
                .SetDelay(delay);
        }
        
        private void OnEnable()
        {
            if (startOnEnable)
            {
                Run();
            }
        }

        private void OnDisable()
        {
            tween?.Kill(true);
            tween = null;
            transform.position = originaPosition;
        }
    }
}