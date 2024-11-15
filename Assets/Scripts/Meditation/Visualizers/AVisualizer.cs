using Meditation.Ui;

namespace Meditation.Visualizers
{
    public abstract class AVisualizer : ABaseElement
    {
        public virtual void Initialize()
        { }
        
        protected override void OnInit()
        {
            LookUp.Get<AVisualizer>().Register(this);
        }

        protected override void OnDeInit()
        {
            LookUp.Get<AVisualizer>().Unregister(this);
        }
    }
}