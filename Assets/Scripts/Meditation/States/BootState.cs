using Cysharp.Threading.Tasks;
using Meditation.Apis.Audio;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;

namespace Meditation.States
{
    public class BootState : AState
    {
        public override UniTask Initialize() => UniTask.CompletedTask;

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ServiceLocator.Get<IAudioEnvironmentManager>().Apply("default");
            ServiceLocator.Get<IUiManager>().HideView(false);
            ServiceLocator.Get<IUiManager>().GetAllViews().ForEach(view=>view.Hide(false));
            ServiceLocator.Get<IUiManager>().ShowView(true);
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