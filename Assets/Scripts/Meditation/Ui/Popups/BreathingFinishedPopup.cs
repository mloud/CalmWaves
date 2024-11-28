using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using OneDay.Core;
using OneDay.Core.Modules.Share;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class BreathingFinishedPopup : UiPopup
    {
        [SerializeField] private AExtendedText breathsCount;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button shareButton;
        [SerializeField] private TextFader textFader;
        protected override void OnInit()
        {
            continueButton.onClick.AddListener(OnContinue);
            shareButton.onClick.AddListener(OnShare);
        }

        protected override UniTask OnOpenStarted(IUiParameter parameter)
        {
            Debug.Assert(parameter != null);
            breathsCount.Set(parameter.GetFirst<FinishedBreathing>().Breaths.ToString());
            textFader.Clear();
            return UniTask.CompletedTask;
        }

        protected override UniTask OnOpenFinished(IUiParameter parameter)
        {
            textFader.Show().Forget();
            return UniTask.CompletedTask;
        }
        private void OnContinue()
        {
            Close().Forget();
        }
        
        private static void OnShare()
        {
            ServiceLocator.Get<IShare>().TakeScreenshotAndShare();
        }
    }
}