using UnityEngine;

namespace Meditation.Ui.Components
{
    public class IntValueChanger : AValueChanger<int>
    {
        [SerializeField] private int increaseStep;
        protected override int IncreaseValue(in int value) => value + increaseStep;

        protected override int DecreaseValue(in int value) => value - increaseStep;
    }
}