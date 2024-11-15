using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class BreathingButton :MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private Button button;
        [SerializeField] private Button infoButton;
        [SerializeField] private LabelWithImage labelWithImage;
        public string Id { get; private set; }

        private Action<string> ButtonAction{ get; set; }
        private IBreathingSettings BreathingSetting { get; set; }
        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
            infoButton.onClick.AddListener(OnInfoClick);
        }

        public void Set(IBreathingSettings breathingSettings, string id, Action<string> action)
        {
            Id = id;
            nameLabel.text = breathingSettings.GetName();
            BreathingSetting = breathingSettings;
            ButtonAction = action;

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
            ServiceLocator.Get<IUiManager>()
                .ShowInfoPopup(BreathingSetting, true, true);
        }
    }
}