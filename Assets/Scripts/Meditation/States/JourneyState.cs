using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Apis;
using Meditation.Data.Breathing;
using Meditation.Ui.Views;
using OneDay.Core;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;


namespace Meditation.States
{
    public class JourneyState : AState
    {
        private JourneyView view;
        private IJourneyManager journeyManager;
        private JourneySettingsDb journeySettingsDb;
        
        public override async UniTask Initialize()
        {
            view = ServiceLocator.Get<IUiManager>().GetView<JourneyView>();
            journeyManager = ServiceLocator.Get<IJourneyManager>();

            journeySettingsDb = (await journeyManager.GetJourneys()).FirstOrDefault();
            
            view.BindAction(view.BackButton, OnBackButtonClicked);
            view.Initialize(journeySettingsDb);
            view.JourneyButtonClicked += OnJourneyButtonClicked;
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        {
            view.MissionsCanvasGroup.alpha = 0;
            var currentMission = await journeyManager.GetProgression(journeySettingsDb.Id);
            currentMission ??= new JourneyProgression
            {
                CurrentProgress = 0,
                JourneyId = journeySettingsDb.Id
            };
            await view.Show(true);
            view.Set(currentMission.CurrentProgress);
            await view.MissionsCanvasGroup.DOFade(1, 1.3f).ToUniTask();
        }

        public override UniTask ExecuteAsync() => UniTask.CompletedTask;

        public override async UniTask ExitAsync()
        {
            await view.Hide(true);
        }
        
        private void OnBackButtonClicked() => 
            StateMachine.SetStateAsync<MenuState>(waitForCurrentStateExit:false).Forget();

        private void OnJourneyButtonClicked(int missionOrder)
        {
            var mission = journeySettingsDb.GetBreathingSettings(missionOrder);
            mission.SetCustomName(journeySettingsDb.Name);
            StateMachine.SetStateAsync<BreathingState>(StateData.Create(
                            ("Settings", mission), 
                            ("Type", "Journey"),
                            ("Data", new JourneyContext(journeySettingsDb.Id, missionOrder))),
                    false)
                
                .Forget();
        }

        public class JourneyContext
        {
            public string JourneyId { get; }
            public int Order { get; }
            public JourneyContext(string journeyId, int order)
            {
                JourneyId = journeyId;
                Order = order;
            }
        }
    }
}