using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;

namespace Meditation.Ui
{
    public class NotificationPopup : UiPopup
    {
        protected override UniTask OnOpenStarted(IUiParameter parameter)
        {
            ServiceLocator.Get<IUiManager>().HideView();
            return UniTask.CompletedTask;
        }

        protected override UniTask OnCloseStarted()
        {
            ServiceLocator.Get<IUiManager>().ShowView();
            return UniTask.CompletedTask;
        }
    }
}