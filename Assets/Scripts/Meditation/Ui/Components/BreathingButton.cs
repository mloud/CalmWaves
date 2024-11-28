using System;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class BreathingButton :MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private Button button;
        [SerializeField] private Button infoButton;
        [SerializeField] private LabelWithImage labelWithImage;
        [SerializeField] private Image icon;
        public string Id { get; private set; }

        private Action<string> ButtonAction{ get; set; }
        private IBreathingSettings BreathingSetting { get; set; }
        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
            infoButton.onClick.AddListener(OnInfoClick);
        }

        public async UniTask Set(IBreathingSettings breathingSettings, string id, Action<string> action)
        {
            Id = id;
            nameLabel.text = breathingSettings.GetName();
            BreathingSetting = breathingSettings;
            ButtonAction = action;

            if (string.IsNullOrEmpty(breathingSettings.GetIcon()))
            {
                icon.enabled = false;
            }
            else
            {
                var asset = await ServiceLocator.Get<IAssetManager>()
                    .GetAssetAsync<Sprite>(breathingSettings.GetIcon());
                icon.sprite = asset.GetReference();
            }

            if (breathingSettings.GetLabel() != null)
            {
                labelWithImage.gameObject.SetActive(true);
                labelWithImage.Set(breathingSettings.GetLabel());
            }
            else
            {
                labelWithImage.gameObject.SetActive(false);
            }
        }
        
        private void OnButtonClick()
        {
            if (ButtonAction == null)
            {
                Debug.LogError("No button action set");
                return;
            }
            ButtonAction(Id);
        }
        
        private void OnInfoClick()
        {
            var request = ServiceLocator.Get<IUiManager>().OpenPopup<InfoPopup>(UiParameter.Create(BreathingSetting));
            request.Popup.BindAction(request.Popup.CloseButton, () => request.Popup.Close(), true);
            request.Popup.ContinueButton.gameObject.SetActive(false);
            request.OpenTask.Forget();
        }
    }
}