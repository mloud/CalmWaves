using System;
using System.Globalization;

namespace Meditation.Core
{
    public static class DateTimeUtils
    {
        public static string GetLocalizedDayName(DayOfWeek day) => 
            CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day);
        
        public static string GetLocalizedDayNameAbbrev(DayOfWeek day) => 
            GetLocalizedDayName(day)[..2];
        
    }
}