using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace OneDay.Core.Modules.Ui
{
    public class PageShowAnimation : AShowAnimation
    {
        private RectTransform rectTransform;

        private Vector2 screenRight;
        private Vector2 screenLeft;
        private Vector2 screenCenter; // Position in the center of the screen

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            screenRight = new Vector2(Screen.width, 0); // Off-screen to the right
            screenLeft = new Vector2(-Screen.width, 0); // Off-screen to the left
            screenCenter = Vector2.zero;
        }

        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1)
        {
            DOTween.Kill(rectTransform);
            rectTransform.anchoredPosition = screenRight * 2; // Start at right
            gameObject.SetActive(true);
            await rectTransform.DOAnchorPos(screenCenter, useSmooth ? duration : 0)
                .SetEase(ease)
                .AsyncWaitForCompletion();
        }

        public override async UniTask Hide(bool useSmooth, float speedMultiplier = 1)
        {
            DOTween.Kill(rectTransform);
            await rectTransform
                .DOAnchorPos(screenLeft * 2, useSmooth ? duration : 0)
                .SetEase(ease);
            gameObject.SetActive(false);
        }
    }
}