using Cysharp.Threading.Tasks;

namespace OneDay.Core.Sm
{
    public interface IState
    {
        StateMachine StateMachine { get; set; }
        UniTask Initialize();
        UniTask EnterAsync(StateData stateData = null);
        UniTask ExecuteAsync();
        UniTask ExitAsync();
    }
}