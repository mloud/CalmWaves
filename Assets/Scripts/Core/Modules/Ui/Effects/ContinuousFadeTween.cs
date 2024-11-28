using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OneDay.Core.Modules.Ui.Effects
{
    public class ContinuousFadeTween : MonoBehaviour
    {
        public float SpeedMultiplier = 1.0f;
      
        [SerializeField] private float endValue;
        [SerializeField] private float delay;
        [SerializeField] private float duration;
        [SerializeField] private Image image;
        private void OnEnable()
        {
            image.DOFade(endValue, duration)
                .SetLoops(-1, LoopType.Yoyo) // Infinite loop with ping-pong effect
                .SetEase(Ease.InOutSine)
                .SetDelay(delay);
        }

        private void OnDisable()
        {
            DOTween.KillAll(transform);
        }
    }
}