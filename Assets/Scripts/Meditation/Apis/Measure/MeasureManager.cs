using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using OneDay.Core;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace Meditation.Apis.Measure
{
    public interface IMeasure
    {
        UniTask SaveBreathingTestResult(BreathingTestResult breathingTestResult);
        IReadOnlyList<(DayOfWeek, TimeSpan)> GetBestResultsThisWeek(string type);
    }
    
    
    public class MeasureManager : MonoBehaviour, IMeasure, IService
    {
        private IDataManager dataManager;
        private Calendar<BreathingTestResult> breathingTestCalendar;
        
        public async UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            breathingTestCalendar = new Calendar<BreathingTestResult>();
            var breathingTests = await dataManager.GetAll<BreathingTestResult>();
            breathingTestCalendar.AddEvents(breathingTests.Select(x=>(x, x.Date)).ToArray());
        }        
    
        public async UniTask SaveBreathingTestResult(BreathingTestResult breathingTestResult)
        {
            await dataManager.Add(breathingTestResult);
            breathingTestCalendar.AddEvent(breathingTestResult, breathingTestResult.Date);
        }

        public IReadOnlyList<(DayOfWeek, TimeSpan)> GetBestResultsThisWeek(string type)
        {
            var result = new List<(DayOfWeek, TimeSpan)>();
            var breathingThisWeek = breathingTestCalendar.GetDataForThisWorkingWeek();
            foreach (var day in breathingThisWeek)
            {
                if (day.data.Count > 0)
                {
                    var ts = day.data.Max(x => x.Tests[type]);
                    result.Add((day.Item1, ts));
                }
                else
                {
                    result.Add((day.Item1, TimeSpan.Zero));
                }
            }
            return result;
        }
 }
}