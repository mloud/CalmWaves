using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class InfoPopup : UiPopup
    {
        public Button ContinueButton => continueButton;
        
        [SerializeField] private Button continueButton;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private BreathingSettingsPanel breathingSettingsPanel;


        protected override UniTask OnOpenStarted(IUiParameter parameter)
        {
            var breathingSettings = parameter.GetFirst<IBreathingSettings>();
            breathingSettingsPanel.Set(breathingSettings);
            titleLabel.text = breathingSettings.GetName();
            textLabel.text = breathingSettings.GetDescription();

            ServiceLocator.Get<IUiManager>().HideRootView().Forget();
            
            return UniTask.CompletedTask;
        }

        protected override UniTask OnCloseStarted()
        {
            ServiceLocator.Get<IUiManager>().ShowRootViews().Forget();   
            return UniTask.CompletedTask;
        }
    }
}