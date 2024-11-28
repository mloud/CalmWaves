using DG.Tweening;
using UnityEngine;

namespace Meditation.Tweens
{
    public class ScaleTween : MonoBehaviour
    {
        [SerializeField] private Vector3 targetValue;
        [SerializeField] private Vector3 fromValue;
        [SerializeField] private float duration;
        [SerializeField] private Ease ease;
        private void OnEnable()
        {
            transform.DOScale(targetValue, duration)
                .SetEase(ease)
                .From(fromValue);
        }
    }
}