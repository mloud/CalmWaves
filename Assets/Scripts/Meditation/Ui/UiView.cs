using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class UiView : UiElement
    {
        public Button BackButton => backButton;
        
        [SerializeField] protected CanvasGroup Cg;
        [SerializeField] protected Button backButton;
        [SerializeField] private float transitionInDuration = 0.5f;
        [SerializeField] private float transitionOutDuration = 0.5f;

        protected override void OnInit()
        { }

        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            Debug.Log($"[UI] Showing view:{gameObject} smooth:{useSmooth}");
            Cg.interactable = true;
            gameObject.SetActive(true);

            if (useSmooth && speedMultiplier > 0)
            {
                await Cg.DOFade(1, transitionInDuration * speedMultiplier)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }
            else
            {
                Cg.alpha = 1.0f;
            }
        }
        public override async UniTask Hide(bool useSmooth, float speedMultiplier = 1.0f)
        {
            Debug.Log($"[UI] Hiding view:{gameObject} smooth:{useSmooth}");
            Cg.interactable = false;

            if (useSmooth && speedMultiplier > 0)
            {
                await Cg.DOFade(0, transitionOutDuration * speedMultiplier)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }
            else
            {
                Cg.alpha = 0;
            }

            gameObject.SetActive(false);   
        }

        public UiView BindAction(Button button, Action action)
        {
            button.onClick.AddListener(()=>action());
            return this;
        }
        public UiView BindAction(Button button, Func<UniTask> asyncAction)
        {
            button.onClick.AddListener(()=>asyncAction().Forget());
            return this;
        }
    }
}