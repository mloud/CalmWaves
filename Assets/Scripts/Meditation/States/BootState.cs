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
            var camera = Camera.main;
            camera.enabled = false;
            await LookUp.Get<SplashView>().GetFirst().Hide(false);
            await LookUp.Get<MenuView>().GetFirst().Hide(false);
            await LookUp.Get<BreathingView>().GetFirst().Hide(false);
            await LookUp.Get<MoodView>().GetFirst().Hide(false);
            
            await LookUp.Get<SplashView>().GetFirst().Show(false);
            await LookUp.Get<SplashView>().GetFirst().Animate(Transit);
            
            return;

            void Transit()
            {
                camera.enabled = true;
                StateMachine.SetStateAsync<MenuState>(waitForCurrentStateExit: false).Forget();
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