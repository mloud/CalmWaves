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
        public TimeSpan Time => new TimeSpan(hours, minutes, 0);
        
        [SerializeField] private TMP_Text dayTimeLabel;
        [SerializeField] private TimeSpanText timeLabel;
        [SerializeField] private CImage image;
        [SerializeField] private CToggle toggle;
        [SerializeField] private Button upButtonHours;
        [SerializeField] private Button downButtonHours;
        [SerializeField] private Button upButtonMinutes;
        [SerializeField] private Button downButtonMinutes;

        private RuntimeOnlyDayTimeNotificationSettings settings;

        private int minutes;
        private int hours;
        
        private void Awake()
        {
            upButtonHours.onClick.AddListener(OnHoursButtonUp);
            downButtonHours.onClick.AddListener(OnHoursButtonDown);
            upButtonMinutes.onClick.AddListener(OnMinutesButtonUp);
            downButtonMinutes.onClick.AddListener(OnMinutesButtonDown);
            toggle.onChange.AddListener(OnSelected);
        }

        public async UniTask Set(RuntimeOnlyDayTimeNotificationSettings settings)
        {
            this.settings = settings;
            
            toggle.SetOn(settings.IsOn, false);
            hours = this.settings.Time.Hours;
            minutes = this.settings.Time.Minutes;
            timeLabel.Set(settings.Time);
            dayTimeLabel.text = this.settings.DefaultSettings.Name;
            await image.SetImage(settings.DefaultSettings.Icon);
        }

        public RuntimeOnlyDayTimeNotificationSettings GetCurrentSettings() => 
            settings;
        
        private void OnHoursButtonUp()
        {
            hours += 1;
            hours %= 24;
         
            timeLabel.Set(Time);
            settings.Time = Time;
        }

        private void OnHoursButtonDown()
        {
            hours -= 1;
            if (hours < 0)
                hours += 24;
            
            timeLabel.Set(Time);
            settings.Time = Time;
        }
        
        private void OnMinutesButtonDown()
        {
            minutes -= 1;
            if (minutes < 0)
                minutes += 60;
            timeLabel.Set(Time);
            settings.Time = Time;
        }

        private void OnMinutesButtonUp()
        {
            minutes += 1;
            minutes %= 60;
            timeLabel.Set(Time);
            settings.Time = Time;
        }
        
        private void OnSelected(bool isSelected)
        {
            settings.IsOn = isSelected;
        }
    }
}