using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Sm;

namespace Meditation.States
{
    public abstract class AState : IState
    {
        public StateMachine StateMachine { get; set; }

        public abstract UniTask Initialize();

        public abstract UniTask EnterAsync(StateData stateData = null);

        public abstract UniTask ExecuteAsync();

        public abstract UniTask ExitAsync();
    }
}