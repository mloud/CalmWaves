using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Meditation.Apis
{
    public interface IBreathingApi
    {
        UniTask<IBreathingSettings> GetBreathingSettingsForCurrentPartOfTheDay();
        UniTask RegisterFinishedBreathing(IBreathingSettings breathingSettings);
        IReadOnlyList<(DayOfWeek, IReadOnlyList<FinishedBreathing>)> GetFinishedBreathingsThisWeek();
        IReadOnlyList<FinishedBreathing> GetFinishedBreathingsToday();
        TimeSpan GetBreathingTime();
        TimeSpan IncreaseBreathingTime();
        TimeSpan DecreaseBreathingTime();
    }
    
    public class BreathingApi : IBreathingApi, IService
    {
        private Calendar<FinishedBreathing> FinishedBreathingCalendar { get; set; }

        private TimeSpan breathingDuration;
        private IDataManager dataManager;
        
        public async UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            
            FinishedBreathingCalendar = new Calendar<FinishedBreathing>();
            breathingDuration = TimeSpan.FromMinutes(3);
            
            var finishedBreathings = await ServiceLocator.Get<IDataManager>().GetAll<FinishedBreathing>();
            finishedBreathings.ToList().ForEach(x=> FinishedBreathingCalendar.AddEvent(x, x.DateTime));
        }
        
        public async UniTask RegisterFinishedBreathing(IBreathingSettings breathingSettings)
        {
            Debug.Assert(breathingSettings != null);
            var finishedBreathing = new FinishedBreathing(breathingSettings);
            
            // save to data
            await ServiceLocator.Get<IDataManager>().Add(finishedBreathing);
            
            // update calendar
            FinishedBreathingCalendar.AddEvent(finishedBreathing, finishedBreathing.DateTime);            
        }

        public TimeSpan GetBreathingTime() => breathingDuration;

        public TimeSpan IncreaseBreathingTime()
        {
            breathingDuration = breathingDuration.Add(TimeSpan.FromMinutes(1));
            if (breathingDuration.TotalMinutes > 20)
            {
                breathingDuration = TimeSpan.FromMinutes(20);
            }
            return breathingDuration;
        }

        public TimeSpan DecreaseBreathingTime()
        {
            breathingDuration = breathingDuration.Subtract(TimeSpan.FromMinutes(1));
            if (breathingDuration.TotalMinutes < 1)
            {
                breathingDuration = TimeSpan.FromMinutes(1);
            }
            return breathingDuration;
        }

        public async UniTask<IBreathingSettings> GetBreathingSettingsForCurrentPartOfTheDay()
        {
            var dailyBreathingSettings = await dataManager.GetDailyBreathingSettings();
            var now = DateTime.Now.TimeOfDay;

            var breathingSettings = dailyBreathingSettings.GetReference().GetAll().ToList();

            var suitableSettings = breathingSettings.Where(x =>
                    now.Hours >= x.GetBreathingTargetTime().FromHour && now.Hours < x.GetBreathingTargetTime().ToHour)
                .ToList();

            if (!suitableSettings.Any())
            {
                suitableSettings = breathingSettings.Where(x => x.GetBreathingTargetTime().AnyTime).ToList();
            }

            Debug.Assert(suitableSettings.Count > 0);
            return suitableSettings[Random.Range(0, suitableSettings.Count)];
        }
        
        public IReadOnlyList<(DayOfWeek, IReadOnlyList<FinishedBreathing>)> GetFinishedBreathingsThisWeek() =>
            FinishedBreathingCalendar.GetDataForThisWorkingWeek();
    
        public IReadOnlyList<FinishedBreathing> GetFinishedBreathingsToday() =>
            FinishedBreathingCalendar.GetDataForToday();
    }
}