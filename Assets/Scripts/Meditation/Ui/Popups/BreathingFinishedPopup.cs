using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using Meditation.Ui.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class BreathingFinishedPopup : UiPopup
    {
        [SerializeField] private AExtendedText breathsCount;
        [SerializeField] private Button continueButton;
        [SerializeField] private TextFader textFader;
        protected override void OnInit()
        {
            continueButton.onClick.AddListener(OnContinue);
        }

        protected override UniTask OnOpenStarted(IUiParameter parameter)
        {
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
    }
}