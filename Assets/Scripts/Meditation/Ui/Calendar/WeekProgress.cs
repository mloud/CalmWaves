using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using UnityEngine;

namespace Meditation.Ui.Calendar
{
    public class WeekProgress : UiElement
    {
        [SerializeField] private List<DayProgress> dayProgress;

        private void Awake()
        {
            LookUp.Get<WeekProgress>().Register(this);
        }

        private void OnDestroy()
        {
            LookUp.Get<WeekProgress>().Unregister(this);
        }
        
        public void Set(IReadOnlyList<(DayOfWeek dayOfWeek, TimeSpan breathingDuration)> currentWeek, TimeSpan requiredDuration)
        {
            foreach (var day in dayProgress)
            {
                var dayTotalBreathing =
                    currentWeek.FirstOrDefault(x => x.dayOfWeek == day.DayOfWeek);
                day.Set(
                    (float)dayTotalBreathing.breathingDuration.TotalSeconds,
                    (float)requiredDuration.TotalSeconds, 
                    DateTime.Today.DayOfWeek == day.DayOfWeek);
            }
        }

        public async UniTask Actualize(DayOfWeek dayOfWeek, TimeSpan totalBreathingTimeToday, TimeSpan requiredBreathingTimes)
        {
            await GetDayProgress(dayOfWeek).Actualize(
                (float)totalBreathingTimeToday.TotalSeconds,
                (float)requiredBreathingTimes.TotalSeconds,
                DateTime.Today.DayOfWeek == dayOfWeek);
        }

        private DayProgress GetDayProgress(DayOfWeek dayOfWeek) => 
            dayProgress.First(x => x.DayOfWeek == dayOfWeek);
    }
}