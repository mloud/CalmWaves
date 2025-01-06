using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace OneDay.Core.Modules.Ui
{
    public class UiElement : ABaseElement
    {
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private float transitionInDuration = 0.5f;
        [SerializeField] private float transitionOutDuration = 0.5f;
        [SerializeField] private AShowAnimation showAnimation;

        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            Debug.Log($"[UI] Showing view:{gameObject} smooth:{useSmooth}");

            if (showAnimation != null)
            {
                await showAnimation.Show(useSmooth, speedMultiplier);
                return;
            }
            
            gameObject.SetActive(true);

            if (useSmooth && speedMultiplier > 0)
            {
                var t = cg.DOFade(1, transitionInDuration * speedMultiplier)
                    .SetEase(Ease.Linear);
                await t.AsyncWaitForCompletion();
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
            if (showAnimation != null)
            {
                await showAnimation.Hide(useSmooth, speedMultiplier);
                return;
            }
            
            
            cg.interactable = false;

            if (useSmooth && speedMultiplier > 0)
            {
                await cg.DOFade(0, transitionOutDuration * speedMultiplier)
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