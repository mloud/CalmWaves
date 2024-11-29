using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Meditation.Data;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class CustomBreathingButton :MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TapAndHoldDetector button;
        [SerializeField] private Button deleteButton;
        [SerializeField] private float deleteButtonDuration = 4;
        public string Id { get; private set; }

        private Action<CustomBreathingSettings> ButtonAction{ get; set; }
        private Action<CustomBreathingSettings> DeleteAction{ get; set; }

        private CustomBreathingSettings BreathingSetting { get; set; }
        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
            deleteButton.onClick.AddListener(OnDeleteClick);
            deleteButton.gameObject.SetVisibleWithFade(false, 0, false).Forget();
            
            button.onLongPress.AddListener(()=>StartCoroutine(ShowDeleteButton()));
        }

        // FULL
        public void Set(CustomBreathingSettings breathingSettings, Action<CustomBreathingSettings> action, Action<CustomBreathingSettings> deleteAction)
        {
            nameLabel.text = breathingSettings.GetName();
            BreathingSetting = breathingSettings;
            ButtonAction = action;
            DeleteAction = deleteAction;
            button.GetComponentInChildren<TextMeshProUGUI>().SetAlpha(1.0f);
        }
        
        //EMPTY
        public void Set()
        {
            Id = "";
            nameLabel.SetTextId(TextIds.STR_RECENT_EXERCISE);
            BreathingSetting = null;
            ButtonAction = null;
            DeleteAction = null;
            button.GetComponentInChildren<TextMeshProUGUI>().SetAlpha(0.5f);
            deleteButton.gameObject.SetVisibleWithFade(false, 0f, false).Forget();
        }

        private IEnumerator ShowDeleteButton()
        {
            deleteButton.gameObject.SetVisibleWithFade(true, 0.5f, false).Forget();
            yield return new WaitForSeconds(deleteButtonDuration);
            deleteButton.gameObject.SetVisibleWithFade(false, 0.5f, false).Forget();
        }


        private void OnDisable()
        {
            deleteButton.gameObject.SetVisibleWithFade(false, 0, false).Forget();
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
        
        private void OnDeleteClick()
        {
            DeleteAction?.Invoke(BreathingSetting);
        }
    }
}