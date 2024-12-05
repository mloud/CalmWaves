using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Data.Notifications;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Notifications;
using Unity.Notifications;
using UnityEngine;

namespace Meditation.Managers
{
    public interface INotificationManager
    {
        UniTask<IEnumerable<RuntimeOnlyDayTimeNotificationSettings>> GetDayTimeNotificationSettings();

        UniTask SaveDayTimeNotificationSettings(
            IEnumerable<RuntimeOnlyDayTimeNotificationSettings> runtimeNotificationSettings);
    }
    
    public class NotificationManager : MonoBehaviour, INotificationManager, IService
    {
        private AsyncCache<IEnumerable<RuntimeOnlyDayTimeNotificationSettings>> settingsCache;
        
        private bool IsInitialized { get; set; }
        public async UniTask Initialize()
        {
            settingsCache =
                new AsyncCache<IEnumerable<RuntimeOnlyDayTimeNotificationSettings>>(GetDayTimeNotificationSettings);
            await settingsCache.Preload();
            IsInitialized = true;
        }

        public async UniTask SaveDayTimeNotificationSettings(IEnumerable<RuntimeOnlyDayTimeNotificationSettings> runtimeNotificationSettings)
        {
            var dataManager = ServiceLocator.Get<IDataManager>();
            foreach (var runtimeSetting  in runtimeNotificationSettings)
            {
                var userSettings = (await dataManager.GetAll<UserDayTimeNotificationSettings>())
                    .Where(x => x.NotificationId == runtimeSetting.DefaultSettings.NotificationId)
                    .ToList();
                Debug.Assert(userSettings.Count <= 1);
                var userSetting = userSettings.FirstOrDefault();
                
                if (runtimeSetting.IsOn)
                {
                    if (userSetting != null)
                    {
                        userSetting.Time = runtimeSetting.Time;
                        await dataManager.Actualize(userSetting);
                    }
                    else
                    {
                        await dataManager.Add(new UserDayTimeNotificationSettings
                        {
                            NotificationId = runtimeSetting.DefaultSettings.NotificationId,
                            Time = runtimeSetting.Time
                        });
                    }
                }
                else
                {
                    userSettings.ForEach(x=>dataManager.Remove<UserDayTimeNotificationSettings>(x.Id));
                }
            }
            settingsCache.Clear();
            await settingsCache.Preload();
        }

        public async UniTask<IEnumerable<RuntimeOnlyDayTimeNotificationSettings>> GetDayTimeNotificationSettings()
        {
            var dataManager = ServiceLocator.Get<IDataManager>();
            
            var notificationDayTimeContentSettings = (await dataManager.GetAll<ContentNotificationSettings>()).First();
            var notificationDayTimeUserSettings =
                (await dataManager.GetAll<UserDayTimeNotificationSettings>()).ToList();

            var runtimeNotificationSettings = new List<RuntimeOnlyDayTimeNotificationSettings>();
            
            foreach (var dayTimeContentSettings in notificationDayTimeContentSettings.DayTimeSettingsList)
            {
                var runtimeOnlyDayTimeNotificationSettings =
                    new RuntimeOnlyDayTimeNotificationSettings();

                var userDayTimeSettings =
                    notificationDayTimeUserSettings.FirstOrDefault(x => x.NotificationId == dayTimeContentSettings.NotificationId);
                runtimeOnlyDayTimeNotificationSettings.IsOn = userDayTimeSettings != null;
                runtimeOnlyDayTimeNotificationSettings.Time = userDayTimeSettings?.Time 
                                                              ?? new TimeSpan(dayTimeContentSettings.DefaultHour, dayTimeContentSettings.DefaultMinute,0);
                runtimeOnlyDayTimeNotificationSettings.DefaultSettings = dayTimeContentSettings;
                runtimeNotificationSettings.Add(runtimeOnlyDayTimeNotificationSettings);
            }
       
            return runtimeNotificationSettings;
        }


        private void OnApplicationPause(bool pauseStatus)
        {
            if (!IsInitialized)
                return;
            
            if (pauseStatus)
            {

                var notificationsApi = ServiceLocator.Get<INotificationsApi>();
                var notificationSettings = settingsCache.GetSync();

                foreach (var runtimeNotification in notificationSettings)
                {
                    if (!runtimeNotification.IsOn)
                        continue;
                    var notification = new Notification
                    {
                        Title = runtimeNotification.DefaultSettings.NotificationTitle,
                        Text = runtimeNotification.DefaultSettings.NotificationText
                    };
                
                    notificationsApi.ScheduleNotification(notification, DateTime.Today + runtimeNotification.Time,
                        true);
                }
            }
        }
    }
}