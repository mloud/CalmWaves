using Cysharp.Threading.Tasks;
using Meditation.Apis.Audio;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace Meditation.States
{
    public class BootState : AState
    {
        public override UniTask Initialize() => UniTask.CompletedTask;

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            ServiceLocator.Get<IUiManager>().HideView(false);
            ServiceLocator.Get<IUiManager>().GetAllViews().ForEach(view=>view.Hide(false));
            ServiceLocator.Get<IUiManager>().GetAllPanels().ForEach(view=>view.Hide(false));

            ServiceLocator.Get<IUiManager>().ShowView(true);

            int introPlayed = PlayerPrefs.GetInt("intro_played", 0);
            if (introPlayed == 1)
            {
                StateMachine.SetStateAsync<MenuState>(StateData.Create(("FadeSkybox", true)),false).Forget();
            }
            else
            {
                PlayerPrefs.SetInt("intro_played", 1);
                PlayerPrefs.Save();
                StateMachine.SetStateAsync<IntroState>(null,false).Forget();   
            }
        }

        public override async UniTask ExecuteAsync()
        {
        }

        public override async UniTask ExitAsync()
        {
        }
    }
}