using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class Popup : UiElement
    {
        public string Id => id;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private Button backButton;
        [SerializeField] private string id;
        [SerializeField] private BreathingSettingsPanel breathingSettingsPanel;
        private void Awake()
        {
            backButton.onClick.AddListener(OnBack);
        }

        public async UniTask Show(IBreathingSettings breathingSettings, bool hasCloseButton)
        {
            backButton.gameObject.SetActive(hasCloseButton);
            gameObject.SetActive(true);
            breathingSettingsPanel.Set(breathingSettings);
            titleLabel.text = breathingSettings.GetName();
            textLabel.text = breathingSettings.GetDescription();
            await DOTween.To(() => cg.alpha, v => cg.alpha = v, 1.0f, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }
        
        public async UniTask Hide()
        {
            await DOTween.To(() => cg.alpha, v => cg.alpha = v, 0.0f, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }
        
        private void OnBack()
        {
            ServiceLocator.Get<IUiManager>().HideInfoPopup();
        }
    }
}