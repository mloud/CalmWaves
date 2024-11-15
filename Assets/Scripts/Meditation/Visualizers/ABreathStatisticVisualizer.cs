using UnityEngine;

namespace Meditation.Visualizers
{
    public abstract class ABreathStatisticVisualizer : MonoBehaviour
    {
        public abstract void Init(int totalBreaths);
        public abstract void SetActual(int actualBreaths);
    }
}