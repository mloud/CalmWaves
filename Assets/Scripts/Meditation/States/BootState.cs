using Cysharp.Threading.Tasks;

namespace Meditation.States
{
    public class BootState : AState
    {
        public override UniTask Initialize() => UniTask.CompletedTask;

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ServiceLocator.Get<IUiManager>().HideRootView(false);
            ServiceLocator.Get<IUiManager>().GetAllViews().ForEach(view=>view.Hide(false));
            ServiceLocator.Get<IUiManager>().ShowRootViews(true);
            StateMachine.SetStateAsync<MenuState>(StateData.Create(("FadeSkybox", true)),false).Forget();   
        }

        public override async UniTask ExecuteAsync()
        {
        }

        public override async UniTask ExitAsync()
        {
        }
    }
}