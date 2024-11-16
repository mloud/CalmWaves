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

        protected override void OnInit()
        {
            Cg.alpha = 0;
            Cg.interactable = false;
        }

        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            Cg.interactable = true;
            gameObject.SetActive(true);
            await Cg.DOFade(1, useSmooth ? 0.5f * speedMultiplier : 0)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }
        
        public override async UniTask Hide(bool useSmooth, float speedMultiplier = 1.0f)
        {
            Cg.interactable = false;
            await Cg.DOFade(0, useSmooth ? 0.5f * speedMultiplier :0)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
            gameObject.SetActive(false);   
        }

        public UiView BindAction(Button button, Action action)
        {
            button.onClick.AddListener(()=>action());
            return this;
        }
        public UiView BindAsyncAction(Button button, Func<UniTask> asyncAction)
        {
            button.onClick.AddListener(()=>asyncAction().Forget());
            return this;
        }
    }
}