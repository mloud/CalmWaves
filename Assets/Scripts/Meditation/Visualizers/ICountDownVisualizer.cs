using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Meditation.Visualizers
{
    public interface ICountDownVisualizer
    {
        UniTask Run(CancellationToken cancellationToken, Action onHideStart);
    }
}