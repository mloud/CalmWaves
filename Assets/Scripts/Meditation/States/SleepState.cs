using Cysharp.Threading.Tasks;
using Meditation.Apis.Audio;
using Meditation.Ui.Audio;
using Meditation.Ui.Views;
using OneDay.Core;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;

namespace Meditation.States
{
    public class SleepState : AState
    {
        private SleepView view;
        private IUiManager uiManager;
        private IAudioEnvironmentManager audioEnvironmentManager;
        public override async UniTask Initialize()
        {
            audioEnvironmentManager = ServiceLocator.Get<IAudioEnvironmentManager>();
            uiManager = ServiceLocator.Get<IUiManager>();
            view = uiManager.GetView<SleepView>();
            view
                .BindAction(view.BackButton, OnBackButtonClicked)
                .BindAction(view.AudioButton, OnAudioClicked);
            
            view.HoursValueChanger.Initialize(0, 0,59);
            view.MinutesValueChanger.Initialize(10, 1,59);
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            audioEnvironmentManager.Apply("sleepDefault");
            await view.Show(true);
        }

        public override async UniTask ExecuteAsync()
        { }

        public override async UniTask ExitAsync()
        {
            audioEnvironmentManager.Save(audioEnvironmentManager.Settings);
            audioEnvironmentManager.Apply("default");
            await view.Hide(true);
        }
      
        private void OnBackButtonClicked() => 
            StateMachine.SetStateAsync<MenuState>(waitForCurrentStateExit:false).Forget();
        
        private async UniTask OnAudioClicked()
        {
            var request = uiManager.OpenPopup<AudioPopup>(null);
            await request.OpenTask;
        }
    }
}