using System;
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
        ISession Session { get; }
        UniTask<IBreathingSettings> GetBreathingSettingsForCurrentPartOfTheDay();
       
       
        TimeSpan GetBreathingTime();
        TimeSpan IncreaseBreathingTime();
        TimeSpan DecreaseBreathingTime();
        TimeSpan GetRequiredBreathingDuration();
    }

    
    public class BreathingApi : IBreathingApi, IService
    {
        public Action<int> TotalBreathCountChanged { get; set; }
        public IHistory History { get; private set; }
        public ISession Session { get; private set; }
    
        private Calendar<FinishedBreathing> Calendar { get; set; }
        private TimeSpan breathingDuration;
        private IDataManager dataManager;
        private IBreathingSettings actualBreathingSettings;
   
       
        public async UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            Calendar = new Calendar<FinishedBreathing>();
            breathingDuration = TimeSpan.FromMinutes(3);
            History = new History(Calendar);
            Session = new BreathingSession(Calendar);
            Session.BreathCountChanged += OnBreathingCountInSessionChanged;

            var finishedBreathings = await ServiceLocator.Get<IDataManager>().GetAll<FinishedBreathing>();
            Calendar.AddEvents(finishedBreathings.Select(x=>(x, x.DateTime)));
            //finishedBreathings.ToList().ForEach(x => Calendar.AddEvent(x, x.DateTime));
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

        private void OnBreathingCountInSessionChanged(int count)
        {
            TotalBreathCountChanged?.Invoke(History.GetTotalBreathCyclesCount() + count);
        }
    }
}