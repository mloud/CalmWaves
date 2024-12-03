using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Managers;
using OneDay.Core;
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
            await ServiceLocator.Get<INotificationManager>()
                .SaveDayTimeNotificationSettings(
                    notificationsDayParts.Select(x => x.GetCurrentSettings()));
        }
    }
}