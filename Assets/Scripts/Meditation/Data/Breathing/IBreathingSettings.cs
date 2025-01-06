namespace Meditation.Data
{
    public interface IBreathingSettings
    {
        string GetName();
        string GetIcon();
        string GetDescription();
        string GetLabel();
        float GetInhaleDuration();
        float GetAfterInhaleDuration();
        float GetExhaleDuration();
        float GetAfterExhaleDuration();
        float GetTotalTime();
        float GetOneBreatheTime();
        int Rounds { get; set; }
    }
}