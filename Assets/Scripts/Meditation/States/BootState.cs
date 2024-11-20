using Cysharp.Threading.Tasks;
using Meditation.Ui;
using Meditation.Ui.Views;
using UnityEngine;

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
            Debug.Log(Time.frameCount);
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