using System;
using Newtonsoft.Json;

namespace Meditation.Apis.Data
{
    public class FinishedBreathing: BaseDataObject
    {
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        public int Rounds { get; set; }
        public TimeSpan BreatheDuration { get; set; }
        public BreathingTiming BreathingTiming { get; set; }

        // Parameterless constructor for Newtonsoft
        [JsonConstructor]
        public FinishedBreathing() { }
        public FinishedBreathing(IBreathingSettings breathingSettings, TimeSpan breatheDuration)
        {
            Name = breathingSettings.GetName();
            Rounds = breathingSettings.Rounds;
            BreathingTiming = breathingSettings.GetBreathingTiming();
            DateTime = DateTime.Now;
            BreatheDuration = breatheDuration;
        }
    }
}