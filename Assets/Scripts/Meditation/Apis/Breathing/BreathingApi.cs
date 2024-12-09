using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Meditation.Apis
{
    public interface IBreathingApi
    {
        Action<int> TotalBreathCountChanged { get; set; }
        Action<int> StreakCountChanged { get; set; }

        IBreathingHistory BreathingHistory {get;}
        UniTask<IBreathingSettings> GetBreathingSettingsForCurrentPartOfTheDay();


        int GetStreak();
        TimeSpan GetBreathingTime();
        TimeSpan IncreaseBreathingTime();
        TimeSpan DecreaseBreathingTime();
        TimeSpan GetRequiredBreathingDuration();
        
        
        // session 
        UniTask StartSession(IBreathingSettings breathingSettings);
        UniTask<FinishedBreathing> FinishSession(float time);
  
        void IncreaseBreathingCountInSession();
        
        
        
    }

    
    public class BreathingApi : IBreathingApi, IService
    {
        public Action<int> TotalBreathCountChanged { get; set; }
        public Action<int> StreakCountChanged { get; set; }
        public IBreathingHistory BreathingHistory { get; private set; }

        private Calendar<FinishedBreathing> finishedBreathingCalendar;
    

        private User user;
        private TimeSpan breathingDuration;
        private IDataManager dataManager;
        private IBreathingSettings actualBreathingSettings;
        private ISession session;
    
        public async UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            finishedBreathingCalendar = new Calendar<FinishedBreathing>();
            breathingDuration = TimeSpan.FromMinutes(3);
            BreathingHistory = new BreathingHistory(finishedBreathingCalendar);
            session = new BreathingSession(this);
            session.BreathCountChanged += OnBreathingCountInSessionChanged;

            var finishedBreathings = await dataManager.GetAll<FinishedBreathing>();
            finishedBreathingCalendar.AddEvents(finishedBreathings.Select(x=>(x, x.DateTime)));

            user = (await dataManager.GetAll<User>()).FirstOrDefault();
            if ((DateTime.Today - user.LastFinishedDay).Days > 1)
            {
                user.Streak = 0;
                user.LastFinishedDay = DateTime.MinValue;
                StreakCountChanged?.Invoke(user.Streak);
                await dataManager.Actualize(user);
            }
        }
        
        public UniTask PostInitialize() => UniTask.CompletedTask;

        public int GetStreak() => user.Streak;
        
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
        public UniTask StartSession(IBreathingSettings breathingSettings)
        {
            session.Start(breathingSettings);
            return UniTask.CompletedTask;
        }

        public async UniTask<FinishedBreathing> FinishSession(float time)
        {
            var finishedBreathing = session.Finish(time);
            if (finishedBreathing == null)
                return null;
            
            // save to data
            await dataManager.Add(finishedBreathing);
            // update calendar
            finishedBreathingCalendar.AddEvent(finishedBreathing, finishedBreathing.DateTime);
           
            if ( BreathingHistory.GetBreathingTimeToday().TotalSeconds >=
                 GetRequiredBreathingDuration().TotalSeconds)
            {
                if (user.LastFinishedDay == DateTime.MinValue || (DateTime.Today - user.LastFinishedDay).Days == 1)
                {
                    user.Streak++;
                    user.LastFinishedDay = DateTime.Today;
                    StreakCountChanged.Invoke(user.Streak);
                    dataManager.Actualize(user);
                }
            }

            return finishedBreathing;
        }

        public void IncreaseBreathingCountInSession() => 
            session.IncreaseBreathingCountInSession();

        private void OnBreathingCountInSessionChanged(int count) => 
            TotalBreathCountChanged?.Invoke(BreathingHistory.GetTotalBreathCyclesCount() + count);
    }
}