using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Managers;
using OneDay.Core;
using OneDay.Core.Modules.Notifications;
using Unity.Notifications;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class NotificationDayPartsContainer : MonoBehaviour
    {
        [SerializeField] private NotificationDayPart dayPartPrefab;
     
        private List<NotificationDayPart> notificationsDayParts;
     
        public async UniTask Refresh()
        {
            notificationsDayParts ??= new List<NotificationDayPart>();
            var settings =
                await ServiceLocator.Get<INotificationManager>().GetDayTimeNotificationSettings();

            foreach (var dayPart in settings)
            {
                var dayPartInstance = notificationsDayParts.FirstOrDefault(x =>
                    x.GetCurrentSettings().DefaultSettings.NotificationId == dayPart.DefaultSettings.NotificationId);
                if (dayPartInstance == null)
                {
                    dayPartInstance = Instantiate(dayPartPrefab, transform);
                    notificationsDayParts.Add(dayPartInstance);
                }
                await dayPartInstance.Set(dayPart);
            }
        }

        public async UniTask Save()
        {
            var settings = notificationsDayParts
                .Select(x => x.GetCurrentSettings())
                .ToList();
            await ServiceLocator.Get<INotificationManager>().SaveDayTimeNotificationSettings(settings);

            if (settings.Any(x => x.IsOn))
            {
                var permission = await ServiceLocator.Get<INotificationsApi>().RequestPermission();
                if (permission == NotificationsPermissionStatus.Granted)
                {
                    Debug.LogError("User accepted permission");
                }
                else if (permission == NotificationsPermissionStatus.Denied)
                {
                    Debug.LogError("User needs to open settings to enable notifications");
                }
            }
        }
    }
}