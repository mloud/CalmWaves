using System;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Measure;
using Meditation.Ui.Chart;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace Meditation.Ui
{
    public class MeasureHistoryPopup : UiPopup
    {
        [SerializeField] private DayTimeSpanChart chartInhale;
        [SerializeField] private DayTimeSpanChart chartExhale;

        protected override void OnInit()
        { }

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            IChartData<DayOfWeek, TimeSpan> chartInhaleData = new DayTimeSpanChartData();
            
            // Inhale
            var bestInhalesThisWeek = ServiceLocator.Get<IMeasure>().GetBestResultsThisWeek("Inhale");
            bestInhalesThisWeek.ForEach(x=>chartInhaleData.Values.Add((x.Item1, x.Item2)));
            chartInhaleData.MaxValue = bestInhalesThisWeek.Max(x => x.Item2);
            chartInhale.Name = "Inhale duration";
            chartInhale.Units = "secs";
            chartInhale.ValueToStringConversion =
                ts => Math.Round(ts.TotalSeconds, 1).ToString(CultureInfo.InvariantCulture);
            chartInhale.Set(chartInhaleData);  
            chartInhale.Select(DateTime.Now.DayOfWeek);
            
            
            
            IChartData<DayOfWeek, TimeSpan> chartExhaleData = new DayTimeSpanChartData();
            // Inhale
            var bestExhalesThisWeek = ServiceLocator.Get<IMeasure>().GetBestResultsThisWeek("Exhale");
            bestExhalesThisWeek.ForEach(x=>chartExhaleData.Values.Add((x.Item1, x.Item2)));
            chartExhaleData.MaxValue = bestExhalesThisWeek.Max(x => x.Item2);
            chartExhale.Name = "Exhale duration";
            chartExhale.Units = "secs";
            chartExhale.ValueToStringConversion =
                ts => Math.Round(ts.TotalSeconds, 1).ToString(CultureInfo.InvariantCulture);
            chartExhale.Set(chartExhaleData);  
            chartExhale.Select(DateTime.Now.DayOfWeek);
            
            ServiceLocator.Get<IUiManager>().HideRootView();
        }
     
        protected override UniTask OnCloseStarted()
        {
            ServiceLocator.Get<IUiManager>().ShowRootViews();
            return UniTask.CompletedTask;
        }
        
        protected override UniTask OnOpenFinished(IUiParameter parameter)
        {
            return UniTask.CompletedTask;
        }
    }
}