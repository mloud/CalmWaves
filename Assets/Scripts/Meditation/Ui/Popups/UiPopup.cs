using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public abstract class UiPopup : UiElement
    {
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private Button closeButton;
        [SerializeField] private bool hasBackButton;
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
            if (closeButton != null)
            {
                closeButton.gameObject.SetActive(hasBackButton);
            }

            await OnOpenStarted(parameter);
            await Show(true);
            await OnOpenFinished(parameter);
            State = PopupState.Open;
        }
        
        public async UniTask Close()
        {
            State = PopupState.Closing;
            await OnClose();
            await Hide(true);
            State = PopupState.Closed;
        }

        #region UiElement
        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            gameObject.SetActive(true);
            await DOTween.To(() => cg.alpha, v => cg.alpha = v, 1.0f, transitionDuration)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }

        public override async UniTask Hide(bool useSmooth, float speedMultiplier = 1.0f)
        {
            await DOTween.To(() => cg.alpha, v => cg.alpha = v, 0.0f, transitionDuration)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }
        #endregion
        
        
        protected virtual UniTask OnOpenStarted(IUiParameter parameter) => UniTask.CompletedTask;
        protected virtual UniTask OnOpenFinished(IUiParameter parameter) => UniTask.CompletedTask;
        protected virtual UniTask OnClose() => UniTask.CompletedTask;
    }
}