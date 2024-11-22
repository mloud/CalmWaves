using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public abstract class UiPopup : UiElement
    {
        public Button CloseButton => closeButton;
        [SerializeField] private Button closeButton;
       
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