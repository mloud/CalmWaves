using System.Threading;
using Cysharp.Threading.Tasks;

namespace Meditation.Visualizers
{
    public abstract class ATotalTimeVisualizer : AVisualizer, IPausable
    {
        public abstract bool IsPaused { get; set; }
        public abstract UniTask Run(float totalTime, CancellationToken cancellationToken);
    }
}