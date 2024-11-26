using Cysharp.Threading.Tasks;

namespace OneDay.Core.Modules.Sm
{
    public class ABaseState : IState
    {
        public StateMachine StateMachine { get; set; }
        public virtual UniTask Initialize() => UniTask.CompletedTask;
        public virtual UniTask EnterAsync(StateData stateData = null) => UniTask.CompletedTask;
        public virtual UniTask ExecuteAsync() => UniTask.CompletedTask;
        public virtual UniTask ExitAsync() => UniTask.CompletedTask;
    }
}