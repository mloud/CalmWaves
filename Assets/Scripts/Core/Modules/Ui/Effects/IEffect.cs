namespace Core.Modules.Ui.Effects
{
    public interface IEffect
    {
        string Id { get; }
        void Run();
        void Stop();
        bool IsPlaying();
    }
}