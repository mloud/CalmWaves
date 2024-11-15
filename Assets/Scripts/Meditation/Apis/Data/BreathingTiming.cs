using System;

namespace Meditation.Apis.Data
{
    [Serializable]
    public class BreathingTiming
    {
        public float InhaleDuration;
        public float AfterInhaleDuration;
        public float ExhaleDuration;
        public float AfterExhaleDuration;
    }
}