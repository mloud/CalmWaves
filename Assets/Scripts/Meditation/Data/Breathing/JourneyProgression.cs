using System;
using OneDay.Core.Modules.Data;

namespace Meditation.Data.Breathing
{
    public class JourneyProgression : BaseDataObject
    {
        public string JourneyId;
        public int CurrentProgress;
        public DateTime LastFinishedTime;

        public static JourneyProgression First(string journeyId) => new()
        {
            JourneyId = journeyId,
            CurrentProgress = 0
        };
    }
}