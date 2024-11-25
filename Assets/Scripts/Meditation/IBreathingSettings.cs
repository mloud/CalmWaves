using Meditation.Apis.Data;

namespace Meditation
{
    public interface IBreathingSettings
    {
        string GetName();
        string GetIcon();
        string GetDescription();
        string GetMusic();
        string GetLabel();
        float GetInhaleDuration();
        float GetAfterInhaleDuration();
        float GetExhaleDuration();
        float GetAfterExhaleDuration();
        float GetTotalTime();
        float GetOneBreatheTime();
        BreathingTiming GetBreathingTiming();
        BreathingTargetTime GetBreathingTargetTime();
        int Rounds { get; set; }
    }
}