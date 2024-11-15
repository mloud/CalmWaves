using Cysharp.Threading.Tasks;
using Meditation.Ui;
using Meditation.Ui.Views;

namespace Meditation.States
{
    public class BootState : AState
    {
        public override UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            await LookUp.Get<MenuView>().GetFirst().Hide(false);
            await LookUp.Get<BreathingView>().GetFirst().Hide(false);
            StateMachine.SetStateAsync<MenuState>().Forget();
        }

        public override async UniTask ExecuteAsync()
        {
        }

        public override async UniTask ExitAsync()
        {
        }
    }
}