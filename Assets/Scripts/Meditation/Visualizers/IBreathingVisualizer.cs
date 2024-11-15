using System.Threading;
using Cysharp.Threading.Tasks;

namespace Meditation.Visualizers
{
    public interface IBreathingVisualizer: IPausable
    {
        void Initialize();
        UniTask Inhale(float duration, CancellationToken cancellationToken);
        UniTask InhaleWait(float duration, CancellationToken cancellationToken);
        UniTask Exhale(float duration, CancellationToken cancellationToken);
        UniTask ExhaleWait(float duration, CancellationToken cancellationToken);
    }
}