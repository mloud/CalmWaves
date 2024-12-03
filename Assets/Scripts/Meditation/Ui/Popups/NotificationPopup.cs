using Cysharp.Threading.Tasks;
using Meditation.Ui.Components;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace Meditation.Ui
{
    public class NotificationPopup : UiPopup
    {
        [SerializeField] private NotificationDayPartsContainer dayPartsContainer;

        public override UniTask Initialize() => UniTask.CompletedTask;

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            ServiceLocator.Get<IUiManager>().HideView();
            await dayPartsContainer.Refresh();
        }

        protected override async UniTask OnCloseStarted()
        {
            await dayPartsContainer.Save();
            ServiceLocator.Get<IUiManager>().ShowView();
        }
    }
}