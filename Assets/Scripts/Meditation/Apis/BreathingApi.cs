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
        Action<int> TotalBreathCountChanged { get; set; }
        
        IHistory History {get;}
        UniTask<IBreathingSettings> GetBreathingSettingsForCurrentPartOfTheDay();
        UniTask StartBreathingSession(IBreathingSettings breathingSettings);
        UniTask FinishBreathingSession(IBreathingSettings breathingSettings, float time);
       
        TimeSpan GetBreathingTime();
        TimeSpan IncreaseBreathingTime();
        TimeSpan DecreaseBreathingTime();
        TimeSpan GetRequiredBreathingDuration();
        void IncreaseBreathingCountInSession();

    }

    public interface IHistory
    {
        IReadOnlyList<(DayOfWeek, TimeSpan)> GetBreathingTimesThisWeek();
        TimeSpan GetBreathingTimeToday();
        IReadOnlyList<FinishedBreathing> GetFinishedBreathingsToday();
        IReadOnlyList<(DayOfWeek, IReadOnlyList<FinishedBreathing>)> GetFinishedBreathingsThisWeek();
        int GetTotalBreathCyclesCount();
    }

    public class History : IHistory
    {
        private Calendar<FinishedBreathing> calendar;
        public History(Calendar<FinishedBreathing> calendar) => this.calendar = calendar;
     
        public IReadOnlyList<(DayOfWeek, TimeSpan)> GetBreathingTimesThisWeek()
        {
            var result = new List<(DayOfWeek, TimeSpan)>();
            var breathingThisWeek = calendar.GetDataForThisWorkingWeek();
            foreach (var day in breathingThisWeek)
            {
                var ts = day.data.Aggregate(TimeSpan.Zero, (acc, x) => acc + x.BreatheDuration);
                result.Add((day.Item1, ts));
            }

            return result;
        }

        public TimeSpan GetBreathingTimeToday() => 
            calendar.GetDataForToday().Aggregate(TimeSpan.Zero, (acc, x) => acc + x.BreatheDuration);

        public IReadOnlyList<FinishedBreathing> GetFinishedBreathingsToday() =>
            calendar.GetDataForToday();
        public IReadOnlyList<(DayOfWeek, IReadOnlyList<FinishedBreathing>)> GetFinishedBreathingsThisWeek() =>
           calendar.GetDataForThisWorkingWeek();

        public int GetTotalBreathCyclesCount() =>
            calendar.GetAllEvents().Sum(x => x.Breaths);
    }
    
    public class BreathingApi : IBreathingApi, IService
    {
        public Action<int> TotalBreathCountChanged { get; set; }
        public IHistory History { get; private set; }
    
        private Calendar<FinishedBreathing> Calendar { get; set; }
        private TimeSpan breathingDuration;
        private IDataManager dataManager;
        private IBreathingSettings actualBreathingSettings;
        private int breathsCountInSession;
        
        public async UniTask Initialize()
        {
            breathsCountInSession = 0;
            dataManager = ServiceLocator.Get<IDataManager>();
            Calendar = new Calendar<FinishedBreathing>();
            breathingDuration = TimeSpan.FromMinutes(3);
            History = new History(Calendar);
            

            var finishedBreathings = await ServiceLocator.Get<IDataManager>().GetAll<FinishedBreathing>();
            finishedBreathings.ToList().ForEach(x => Calendar.AddEvent(x, x.DateTime));
        }

        public UniTask StartBreathingSession(IBreathingSettings breathingSettings)
        {
            Debug.Assert(actualBreathingSettings==null, "There is still opened breathign session");
            actualBreathingSettings = breathingSettings;
            breathsCountInSession = 0;
            return UniTask.CompletedTask;
        }

        public void IncreaseBreathingCountInSession()
        {
            breathsCountInSession++;
            TotalBreathCountChanged?.Invoke(History.GetTotalBreathCyclesCount() + breathsCountInSession);
        }
        
        public async UniTask FinishBreathingSession(IBreathingSettings breathingSettings, float time)
        {
            Debug.Assert(breathingSettings != null);
            Debug.Log($"RegisterFinishedBreathing duration {time}");
            if (actualBreathingSettings == null)
            {
                Debug.Log("No actual breathing registered");
                return;
            }

            if (time <= 0)
            {
                Debug.Log("Zero breathing time will not be registered");
                return;
            }

            Debug.Assert(breathingSettings == actualBreathingSettings);
            var finishedBreathing = new FinishedBreathing(breathingSettings, TimeSpan.FromSeconds(time), breathsCountInSession);

            // save to data
            await ServiceLocator.Get<IDataManager>().Add(finishedBreathing);
            actualBreathingSettings = null;
            // update calendar
            Calendar.AddEvent(finishedBreathing, finishedBreathing.DateTime);
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
            var settings = suitableSettings[Random.Range(0, suitableSettings.Count)];

            // override rounds here
            settings.Rounds = Mathf.CeilToInt((float)GetBreathingTime().TotalSeconds / settings.GetOneBreatheTime());

            return settings;
        }
      
        public TimeSpan GetRequiredBreathingDuration() => TimeSpan.FromMinutes(5);
    }
}