using System;
using Cysharp.Threading.Tasks;
using Meditation.Data;
using OneDay.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class CustomBreathingButton :MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private Button button;
        public string Id { get; private set; }

        private Action<CustomBreathingSettings> ButtonAction{ get; set; }
        private CustomBreathingSettings BreathingSetting { get; set; }
        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }
 
        // FULL
        public void Set(CustomBreathingSettings breathingSettings,  Action<CustomBreathingSettings> action)
        {
            nameLabel.text = breathingSettings.GetName();
            BreathingSetting = breathingSettings;
            ButtonAction = action;
            button.interactable = true;
            button.GetComponentInChildren<TextMeshProUGUI>().SetAlpha(1.0f);
        }
        
        //EMPTY
        public void Set()
        {
            Id = "";
            nameLabel.SetTextId(TextIds.STR_RECENT_EXERCISE);
            BreathingSetting = null;
            ButtonAction = null;
            button.interactable = false;
            button.GetComponentInChildren<TextMeshProUGUI>().SetAlpha(0.5f);
        }
        
        private void OnButtonClick()
        {
            if (ButtonAction == null)
            {
                Debug.LogError("No button action set");
                return;
            }
            ButtonAction(BreathingSetting);
        }
    }
}