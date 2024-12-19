using Cysharp.Threading.Tasks;
using Meditation.Apis.Audio;
using Meditation.Apis.Visual;
using Meditation.Ui.Views;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;

namespace Meditation.States
{
    public class IntroState : AState
    {
        private IntroView view;
        public override UniTask Initialize()
        {
            view = ServiceLocator.Get<IUiManager>().GetView<IntroView>();
            return UniTask.CompletedTask;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ServiceLocator.Get<IVisualEnvironmentManager>().FadeInSkybox();
            ServiceLocator.Get<IAudioEnvironmentManager>().Apply("intro");
            view.Parts.ForEach(x=>x.SetVisibleWithFade(false, 0, true).Forget());
            await view.Show(false);
            await UniTask.WaitForSeconds(2.0f);
            for (int i = 0; i < view.Parts.Count; i++)
            {
                await view.Parts[i].SetVisibleWithFade(true, 2.0f, true);
                await UniTask.WaitForSeconds(3.0f);
                await view.Parts[i].SetVisibleWithFade(false, 2.0f, true);
            }
            StateMachine.SetStateAsync<MenuState>(null, false).Forget();
            //StateMachine.SetStateAsync<MenuState>(StateData.Create(("FadeSkybox", true)), false).Forget();
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;

        public override async UniTask ExitAsync()
        {
            await view.Hide(true);
        }
    }
}