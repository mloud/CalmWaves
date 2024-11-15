using DG.Tweening;
using UnityEngine;

namespace Meditation.Tweens
{
    public class PulsateTween : MonoBehaviour
    {
        [SerializeField] private Vector3 endValue;
        [SerializeField] private float duration;
        [SerializeField] private float delay;
        private void OnEnable()
        {
            transform.DOScale(endValue, duration).SetLoops(-1, LoopType.Yoyo) // Infinite loop with ping-pong effect
                .SetEase(Ease.InOutSine).From(Vector3.one).SetDelay(delay);
        }

        private void OnDisable()
        {
            DOTween.KillAll(transform);
        }
    }
}