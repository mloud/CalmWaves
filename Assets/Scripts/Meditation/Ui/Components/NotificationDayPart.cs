using System;
using Cysharp.Threading.Tasks;
using Meditation.Data.Notifications;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class NotificationDayPart : MonoBehaviour
    {
        public bool IsOn => toggle.IsOn;
        public TimeSpan Time => timeSpan;
        
        [SerializeField] private TMP_Text dayTimeLabel;
        [SerializeField] private TimeSpanText timeLabel;
        [SerializeField] private CImage image;
        [SerializeField] private CToggle toggle;
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;

        private TimeSpan timeSpan;
        private RuntimeOnlyDayTimeNotificationSettings settings;
        private void Awake()
        {
            upButton.onClick.AddListener(OnButtonUp);
            downButton.onClick.AddListener(OnButtonDown);
            toggle.onChange.AddListener(OnSelected);
        }

        public async UniTask Set(RuntimeOnlyDayTimeNotificationSettings settings)
        {
            this.settings = settings;
            
            toggle.SetOn(settings.IsOn, false);
            timeSpan = settings.Time;
            timeLabel.Set(settings.Time);
            dayTimeLabel.text = this.settings.DefaultSettings.Name;
            await image.SetImage(settings.DefaultSettings.Icon);
        }

        public RuntimeOnlyDayTimeNotificationSettings GetCurrentSettings() => 
            settings;
        
        private void OnButtonUp()
        {
            timeSpan = timeSpan.Add(TimeSpan.FromHours(1));
            timeLabel.Set(timeSpan);
            settings.Time = timeSpan;
        }

        private void OnButtonDown()
        {
            timeSpan = timeSpan.Add(-TimeSpan.FromHours(1));
            timeLabel.Set(timeSpan);
            settings.Time = timeSpan;
        }
        
        private void OnSelected(bool isSelected)
        {
            settings.IsOn = isSelected;
        }
    }
}