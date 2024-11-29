using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;

namespace Meditation.Ui
{
    public class BreathingHelpPopup : UiPopup
    {
        protected override UniTask OnOpenStarted(IUiParameter parameter)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask OnCloseStarted()
        {
            return UniTask.CompletedTask;
        }
    }
}