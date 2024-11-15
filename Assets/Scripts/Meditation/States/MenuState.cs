using System;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Ui;
using Meditation.Ui.Views;

namespace Meditation.States
{
    public class MenuState : AState
    {
        private MenuView menuView;
        private AddressableAsset<BreathingSettingsDb> breathingDbAsset;
        private IBreathingApi breathingApi;
        public override async UniTask Initialize()
        {
            breathingApi = ServiceLocator.Get<IBreathingApi>();
            breathingDbAsset = await ServiceLocator.Get<IDataManager>().GetBreathingSettings();
            var finishedBreathings = ServiceLocator.Get<IBreathingApi>().GetFinishedBreathingsThisWeek();
            
            menuView = LookUp.Get<MenuView>().GetFirst();
            await menuView.InitializeBreathingButtons(breathingDbAsset.GetReference().GetAll() , OnMenuButtonClicked);
            await menuView.InitializeWeekCalendar(finishedBreathings);
            await menuView.InitializeTimer();

            menuView
                .BindAsyncAction(menuView.StartButton, OnStartClick)
                .BindAsyncAction(menuView.AiButton, OnAiClicked);
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        { 
            await menuView.Show(true);
            if (stateData != null)
            {
                var finishedBreathing = stateData.GetValue<bool>(StateDataKeys.BreathingFinished);
                if (finishedBreathing)
                {
                    await menuView.ActualizeWeekCalendar(DateTime.Today.DayOfWeek, breathingApi.GetFinishedBreathingsToday());
                }
            }
        }

        public override async UniTask ExecuteAsync()
        {
            
        }

        public override async UniTask ExitAsync()
        {
            await menuView.Hide(true);
        }
        
        private async UniTask OnStartClick()
        {
            var settings = await breathingApi.GetBreathingSettingsForCurrentPartOfTheDay();
            
            StateMachine.SetStateAsync<BreathingState>(
                    StateData.Create(("Settings", settings)), false)
                .Forget();
        }
      
        private void OnMenuButtonClicked(string id) =>
            StateMachine.SetStateAsync<BreathingState>(
                    StateData.Create(("Settings", breathingDbAsset.GetReference().Get(id))), false)
                .Forget();

        private async UniTask OnAiClicked() => 
            await StateMachine.SetStateAsync<MoodState>(waitForCurrentStateExit: false);
    }
}