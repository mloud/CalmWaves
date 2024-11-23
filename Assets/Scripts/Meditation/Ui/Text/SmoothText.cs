using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Meditation.Ui.Text
{
    public class SmoothText : AExtendedText
    {
        public Mode TransitionMode;
        public float EffectDuration = 0.2f;

        public string text
        {
            get => text1.text;
            set => Set(value);
        }
        
        [SerializeField] private TextMeshProUGUI text1;
        [SerializeField] private TextMeshProUGUI text2;
       
        private Tween currentTween;
        private Sequence sequence;


        private float originalAlpha;
        private string targetText;
        
        public enum Mode
        {
            None,
            Fade,
            Blend
        }

        private void Awake()
        {
            originalAlpha = text1.alpha;
        }

        public override void Set(string text)
        {
            if (!gameObject.activeInHierarchy)
            {
                SetTextWithoutTransition(text);
                return;
            }

            targetText = text;
            
            switch (TransitionMode)
            {
                case Mode.None:
                    SetTextWithoutTransition(text);
                    break;
                case Mode.Fade:
                    SetTextWithFade(text);
                    break;
                case Mode.Blend:
                    SetTextWithBlend(text);
                    break;
            }
        }

        private void SetTextWithoutTransition(string text)
        {
            text1.alpha = 1.0f;
            text1.text = text;
            if (text2 != null)
            {
                text2.enabled = false;
            }
        }
        
        private void SetTextWithFade(string text)
        {
            sequence?.Kill(true);
            sequence = DOTween.Sequence();
            if (text2 != null)
            {
                text2.enabled = false;
            }

            if (!string.IsNullOrEmpty(text1.text))
            {
                sequence.Append(text1.DOFade(0, EffectDuration).SetEase(Ease.Linear));
            }
            sequence.AppendCallback(() => text1.text = text);
            sequence.Append(text1.DOFade(originalAlpha, EffectDuration).SetEase(Ease.Linear));
        }

        private void SetTextWithBlend(string text)
        {
            sequence?.Kill(true);
            sequence = DOTween.Sequence();

            text2.enabled = true;
            text2.text = text;

            sequence.Append(text1.DOFade(0, EffectDuration).SetEase(Ease.Linear));
            sequence.Join(text2.DOFade(originalAlpha, EffectDuration).SetEase(Ease.Linear));
            sequence.AppendCallback(() => (text1, text2) = (text2, text1));
        }

        private void OnDisable()
        {
            if (sequence == null) return;
            
            if (sequence.IsActive())
            {
                sequence.Complete(true);
            }
            sequence.Kill();
            sequence = null;

            if (targetText != null)
            {
                SetTextWithoutTransition(targetText);
                targetText = null;
            }
        }
    }
}