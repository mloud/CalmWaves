using System;
using System.Collections.Generic;
using OneDay.Core.Modules.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meditation.Data.Notifications
{
    #region Version content
    [Serializable]
    public class NotificationDayTimeSettings
    {
        public string NotificationId;
        public string Name;
        public string Icon;
        public int DefaultHour;
        public int DefaultMinute;
        public bool DefaultState;
        public string NotificationTitle;
        public string NotificationText;
    }
    
    [Serializable]
    public class ContentNotificationSettings : BaseDataObject
    {
        public List<NotificationDayTimeSettings> DayTimeSettingsList;
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NotificationSettings", order = 1)]

    public class NotificationSettingsTable : ScriptableObjectTable<ContentNotificationSettings>
    { }
    
    #endregion
    
    #region User settings
    
    // class used to save notification settings
    public class UserDayTimeNotificationSettings : BaseDataObject
    {
        public string NotificationId;
        public TimeSpan Time;
    }

    public class RuntimeOnlyDayTimeNotificationSettings
    {
        public NotificationDayTimeSettings DefaultSettings;
        public bool IsOn;
        public TimeSpan Time;
    }

    #endregion
}