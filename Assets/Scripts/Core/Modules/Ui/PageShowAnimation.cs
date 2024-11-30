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
            rectTransform.anchoredPosition = screenRight * 2; // Start at right
            await rectTransform.DOAnchorPos(screenCenter, duration).SetEase(ease).AsyncWaitForCompletion();
            
            //await rectTransform.DOMoveX(0, duration)
            //    .SetEase(ease)
             //   .From(Screen.width)
             //   .AsyncWaitForCompletion();
        }

        public override async UniTask Hide(bool useSmooth, float speedMultiplier = 1)
        {
            await rectTransform.DOAnchorPos(screenLeft * 2, duration).SetEase(ease).AsyncWaitForCompletion();
            
        }
            // await transform.DOMoveX(0 - ((RectTransform)transform).rect.width, duration)
            //     .SetEase(ease)
            //     .AsyncWaitForCompletion();
        
    }
}