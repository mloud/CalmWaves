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

        private const int breathingsPerDay = 3;
        
        private void Awake()
        {
            LookUp.Get<WeekProgress>().Register(this);
        }

        private void OnDestroy()
        {
            LookUp.Get<WeekProgress>().Unregister(this);
        }
        
        public void Set(IReadOnlyList<(DayOfWeek dayOfWeek, IReadOnlyList<FinishedBreathing>)> currentWeek)
        {
            foreach (var day in dayProgress)
            {
                var finishedBreathings =
                    currentWeek.FirstOrDefault(x => x.dayOfWeek == day.DayOfWeek);
                day.Set(
                    finishedBreathings.Item2?.Count ?? 0, 
                    breathingsPerDay, 
                    DateTime.Today.DayOfWeek == day.DayOfWeek);
            }
        }

        public async UniTask Actualize(DayOfWeek dayOfWeek, IReadOnlyList<FinishedBreathing> finishedBreathings)
        {
            await GetDayProgress(dayOfWeek).Actualize(
                finishedBreathings?.Count ?? 0, 
                breathingsPerDay,DateTime.Today.DayOfWeek == dayOfWeek);
        }

        private DayProgress GetDayProgress(DayOfWeek dayOfWeek) => 
            dayProgress.First(x => x.DayOfWeek == dayOfWeek);
    }
}