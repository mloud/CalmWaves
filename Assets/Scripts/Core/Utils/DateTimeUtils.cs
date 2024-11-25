using System;
using System.Globalization;

namespace Meditation.Core.Utils
{
    public static class DateTimeUtils
    {
        public static string GetLocalizedDayName(DayOfWeek day) => 
            CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day);
        
        public static string GetLocalizedDayNameAbbrev(DayOfWeek day) => 
            GetLocalizedDayName(day)[..2];

        public static string GetTimespanTo_MSS_String(TimeSpan timeSpan) =>
            $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        
        public static string GetTime(TimeSpan timeSpan) => 
            $"{timeSpan.Minutes}m {timeSpan.Seconds:D2}s";
    }
}