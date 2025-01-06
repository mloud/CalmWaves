using System;
using Meditation.Data;
using Newtonsoft.Json;
using OneDay.Core.Modules.Data;

namespace Meditation.Apis.Data
{
    public class FinishedBreathing: BaseDataObject
    {
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        public TimeSpan BreatheDuration { get; set; }
        public int Breaths { get; set; }

        // Parameterless constructor for Newtonsoft
        [JsonConstructor]
        public FinishedBreathing() { }
        public FinishedBreathing(IBreathingSettings breathingSettings, TimeSpan breatheDuration, int breaths)
        {
            Name = breathingSettings.GetName();
            DateTime = DateTime.Now;
            BreatheDuration = breatheDuration;
            Breaths = breaths;
        }
    }
}