using System;
using System.Collections.Generic;

namespace Meditation.Apis.Data
{
    public static class CalendarExtensions
    {
        public static IReadOnlyList<(DayOfWeek dayInWeek, IReadOnlyList<TData> data)> GetDataForThisWorkingWeek<TData>(this Calendar<TData> calendar)
        {
            int daysAfterMonday = (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysAfterMonday < 0) daysAfterMonday += 7;  // Adjust if we're already in the week

            var startOfWeek = DateTime.Today.AddDays(-daysAfterMonday);
            var endOfWeek = startOfWeek.AddDays(6);  // Sunday of the same week

            var result = new List<(DayOfWeek, IReadOnlyList<TData>)>();
            for (var date = startOfWeek; date <= endOfWeek; date = date.AddDays(1))
            {
               result.Add((date.DayOfWeek, calendar.GetEvents(date)));
            }

            return result;
        }
        
        public static IReadOnlyList<TData> GetDataForToday<TData>(this Calendar<TData> calendar)=>
            calendar.GetEvents(DateTime.Today);
        
    }
}