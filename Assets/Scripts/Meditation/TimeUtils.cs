using System;

namespace Meditation
{
    public static class TimeUtils
    {
        public static string GetTime(float seconds) => 
            TimeSpan.FromSeconds(seconds).ToString(@"m\:ss");
    }
}