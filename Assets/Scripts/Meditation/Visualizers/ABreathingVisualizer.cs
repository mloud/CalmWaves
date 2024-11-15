using System.Threading;
using Cysharp.Threading.Tasks;

namespace Meditation.Visualizers
{
    public abstract class ABreathingVisualizer : AVisualizer, IBreathingVisualizer
    {
        public abstract bool IsPaused { get; set; }
        public abstract UniTask Inhale(float duration, CancellationToken cancellationToken);
        public abstract UniTask InhaleWait(float duration, CancellationToken cancellationToken);
        public abstract UniTask Exhale(float duration, CancellationToken cancellationToken);
        public abstract UniTask ExhaleWait(float duration, CancellationToken cancellationToken);
    }
}