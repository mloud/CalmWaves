using System;
using System.Collections.Generic;
using System.Linq;
using Meditation.Apis.Data;

namespace Meditation.Apis
{
    public interface IBreathingHistory
    {
        IReadOnlyList<(DayOfWeek, TimeSpan)> GetBreathingTimesThisWeek();
        TimeSpan GetBreathingTimeToday();
        IReadOnlyList<FinishedBreathing> GetFinishedBreathingsToday();
        IReadOnlyList<(DayOfWeek, IReadOnlyList<FinishedBreathing>)> GetFinishedBreathingsThisWeek();
        int GetTotalBreathCyclesCount();
    }

    public class BreathingHistory : IBreathingHistory
    {
        private Calendar<FinishedBreathing> calendar;
        public BreathingHistory(Calendar<FinishedBreathing> calendar) => this.calendar = calendar;
     
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
}