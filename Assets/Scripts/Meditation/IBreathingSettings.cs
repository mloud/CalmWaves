using Meditation.Apis.Data;

namespace Meditation
{
    public interface IBreathingSettings
    {
        string GetName();
        string GetDescription();
        string GetMusic();
        string GetLabel();
        float GetInhaleDuration();
        float GetAfterInhaleDuration();
        float GetExhaleDuration();
        float GetAfterExhaleDuration();
        float GetTotalTime();
        BreathingTiming GetBreathingTiming();
        BreathingTargetTime GetBreathingTargetTime();
        int Rounds();
    }
}