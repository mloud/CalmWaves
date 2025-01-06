using Cysharp.Threading.Tasks;
using Meditation.Data;
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
       
        [SerializeField] private Button helpButton;
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

            ServiceLocator.Get<IUiManager>().HideView().Forget(); 
            //BindAction(helpButton, OnHelp);
            return UniTask.CompletedTask;
        }

        protected override UniTask OnCloseStarted()
        {
            //helpButton.onClick.RemoveAllListeners();
            ServiceLocator.Get<IUiManager>().ShowView().Forget();   
            return UniTask.CompletedTask;
        }
        
        private async UniTask OnHelp()
        {
            var request = ServiceLocator.Get<IUiManager>().OpenPopup<BreathingHelpPopup>(null);
            Hide(true).Forget();
            request.OpenTask.Forget();
            await request.WaitForCloseStarted();
            Show(true).Forget();
        }
    }
}