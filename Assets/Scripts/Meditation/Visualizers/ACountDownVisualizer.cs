using System;
using System.Threading;
using Cysharp.Threading.Tasks;


namespace Meditation.Visualizers
{
    public abstract class ACountDownVisualizer : AVisualizer, ICountDownVisualizer
    {
        public abstract UniTask Run(CancellationToken cancellationToken, Action onHideStart);
    }
}