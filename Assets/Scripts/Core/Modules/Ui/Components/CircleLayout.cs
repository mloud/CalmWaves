using UnityEngine;

namespace OneDay.Core.Modules.Ui.Components
{
    public class CircleLayout : CustomLayout
    {
        [SerializeField] private WatchedField<float> spaceBetween;
        [SerializeField] private WatchedField<float> radius;
        [SerializeField] private Direction direction;
        
        public enum Direction
        {
            CounterClockwise,
            Clockwise
        }
        
        protected override void Layout()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                child.localPosition = new Vector3(0, radius.Value);
                child.RotateAround(
                    transform.position, 
                    new Vector3(0,0,1), 
                    (direction == Direction.CounterClockwise ? 1 : -1) * i * spaceBetween.Value);
                child.localEulerAngles = Vector3.zero;
            }
        }

        protected override bool OnDetectChangesInProperties()
        {
            var spaceChanged = spaceBetween.ProcessChange();
            var radiusChanged = radius.ProcessChange();
            return spaceChanged || radiusChanged;
        }
    }
}