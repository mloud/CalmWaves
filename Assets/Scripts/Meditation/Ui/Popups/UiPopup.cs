using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public abstract class UiPopup : UiElement
    {
        public Button CloseButton => closeButton;
        
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private Button closeButton;
        [SerializeField] private float transitionDuration = 0.5f;
       
        public enum PopupState
        {
            Opening,
            Open,
            Closing,
            Closed
        }
        public PopupState State { get; private set; }
        
        public async UniTask Open(IUiParameter parameter)
        {
            State = PopupState.Opening;
            if (closeButton != null && closeButton.gameObject.activeSelf)
            {
                closeButton.onClick.AddListener(() => OnCloseButton().Forget());
            }

            await OnOpenStarted(parameter);
            await Show(true);
            await OnOpenFinished(parameter);
            State = PopupState.Open;
        }

        protected virtual async UniTask OnCloseButton()
        {
            await Close();
        }

        public async UniTask Close()
        {
            State = PopupState.Closing;
            await OnCloseStarted();
            await Hide(true);
            await OnCloseFinished();
            State = PopupState.Closed;
        }

        #region UiElement

        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            gameObject.SetActive(true);
            if (useSmooth)
            {
                await DOTween.To(() => cg.alpha, v => cg.alpha = v, 1.0f, transitionDuration)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }
            cg.alpha = 1.0f;
        }

        public override async UniTask Hide(bool useSmooth, float speedMultiplier = 1.0f)
        {
            if (useSmooth)
            {
                await DOTween.To(() => cg.alpha, v => cg.alpha = v, 0.0f, transitionDuration)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }

            cg.alpha = 0;
            gameObject.SetActive(false);
        }
        #endregion
        
        
        public UiPopup BindAction(Button button, Action action, bool removeAllListeners = true)
        {
            if (removeAllListeners)
            {
                button.onClick.RemoveAllListeners();
            }
            button.onClick.AddListener(()=>action());
            button.gameObject.SetActive(true);
            return this;
        }
        
        protected virtual UniTask OnOpenStarted(IUiParameter parameter) => UniTask.CompletedTask;
        protected virtual UniTask OnOpenFinished(IUiParameter parameter) => UniTask.CompletedTask;
        protected virtual UniTask OnCloseStarted() => UniTask.CompletedTask;
        protected virtual UniTask OnCloseFinished() => UniTask.CompletedTask;
    }
}