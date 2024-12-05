using System;
using Cysharp.Threading.Tasks;
using Unity.Notifications;
using UnityEngine;

namespace OneDay.Core.Modules.Notifications
{
    public interface INotificationsApi
    {
        public Action<Notification> ForegroundNotificationReceived { get; set; }
        Notification? NotificationUsedToOpenApplication { get; }
        void ScheduleNotification(Notification notification, TimeSpan after);
        void ScheduleNotification(Notification notification, DateTime time, bool repeat);
        UniTask<NotificationsPermissionStatus> RequestPermission();
    }

    [Serializable]
    public class NotificationSettings
    {
        public string AndroidChannelId = "default";
        public string AndroidChannelName = "Notifications";
        public string AndroidChannelDescription = "Main notifications";
    }
    
    public class NotificationsApi : MonoBehaviour, IService, INotificationsApi
    {
        public Action<Notification> ForegroundNotificationReceived { get; set; }

        public Notification? NotificationUsedToOpenApplication => 
            NotificationCenter.LastRespondedNotification;
        
        [SerializeField] private bool cancelAllNotificationsAfterStart;
        [SerializeField] private NotificationSettings channelSettings;
        public UniTask Initialize()
        {
            var args = NotificationCenterArgs.Default;
            args.AndroidChannelId = channelSettings.AndroidChannelId;
            args.AndroidChannelName = channelSettings.AndroidChannelName;
            args.AndroidChannelDescription = channelSettings.AndroidChannelDescription;
            NotificationCenter.Initialize(args);

            if (cancelAllNotificationsAfterStart)
            {
                NotificationCenter.CancelAllScheduledNotifications();
                NotificationCenter.CancelAllDeliveredNotifications();
            }
            NotificationCenter.OnNotificationReceived += NotificationReceivedWhileApplicationRunning;
            return UniTask.CompletedTask;
        }

        public void ScheduleNotification(Notification notification, TimeSpan after)
        {
            NotificationCenter.ScheduleNotification(notification, new NotificationIntervalSchedule(after));
        }

        public void ScheduleNotification(Notification notification, DateTime time, bool repeat)
        {
            Debug.Log($"Scheduling notification {notification.Title} to {time}");
            NotificationCenter.ScheduleNotification(notification, 
                new NotificationDateTimeSchedule(time, repeat 
                    ? NotificationRepeatInterval.Daily 
                    : NotificationRepeatInterval.OneTime));
        }

        public async UniTask<NotificationsPermissionStatus> RequestPermission()
        {
            var request = NotificationCenter.RequestPermission();

            while (request.Status == NotificationsPermissionStatus.RequestPending)
                await UniTask.Yield(); 
            
            return request.Status;
        }
        
        private void NotificationReceivedWhileApplicationRunning(Notification notification)
        {
            Debug.Log($"Received notification when application running with title: {notification.Title}");
            ForegroundNotificationReceived?.Invoke(notification);
        }
    }
}