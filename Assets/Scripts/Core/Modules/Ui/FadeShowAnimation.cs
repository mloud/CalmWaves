using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace OneDay.Core.Modules.Ui
{
    public class FadeShowAnimation : AShowAnimation
    {
        [SerializeField] private CanvasGroup cg;
        
        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            Debug.Log($"[UI] Showing view:{gameObject} smooth:{useSmooth}");
            gameObject.SetActive(true);

            if (useSmooth && speedMultiplier > 0)
            {
                await cg.DOFade(1, duration * speedMultiplier)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }
            else
            {
                cg.alpha = 1.0f;
            }
            cg.interactable = true;
        }

        public override async UniTask Hide(bool useSmooth, float speedMultiplier = 1.0f)
        {
            Debug.Log($"[UI] Hiding view:{gameObject} smooth:{useSmooth}");
            cg.interactable = false;

            if (useSmooth && speedMultiplier > 0)
            {
                await cg.DOFade(0, duration * speedMultiplier)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }
            else
            {
                cg.alpha = 0;
            }

            gameObject.SetActive(false);   
        }
    }
}