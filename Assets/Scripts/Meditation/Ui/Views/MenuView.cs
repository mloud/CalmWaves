using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using Meditation.Ui.Calendar;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class MenuView : UiView
    {
        public Button StartButton => startButton;
        public Button AiButton => aiButton;
        
        [SerializeField] private Transform container;
        [SerializeField] private AssetReferenceGameObject menuButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button aiButton;
        [SerializeField] private WeekProgress weekProgress;
        [SerializeField] private Timer.Timer timer;
        
        private List<GameObject> breathingButtons;

        protected override void OnInit()
        {
            Cg.alpha = 0;
            LookUp.Get<MenuView>().Register(this);
        }
 
        public async UniTask InitializeBreathingButtons(IEnumerable<IBreathingSettings> settings, Action<string> menuButtonClicked)
        {
            breathingButtons = new List<GameObject>();
            foreach (var setting in settings)
            {
                var button = await menuButton.InstantiateAsync(container);
                breathingButtons.Add(button);
                button.GetComponent<BreathingButton>().Set(setting, setting.GetName(), menuButtonClicked);
            }
        }
   
        public UniTask InitializeStartButton(Action menuButtonClicked)
        {
            startButton.onClick.AddListener(()=>menuButtonClicked());
            return UniTask.CompletedTask;
        }

        public UniTask InitializeWeekCalendar(IReadOnlyList<(DayOfWeek dayOfWeek, IReadOnlyList<FinishedBreathing>)> finisedBreathings)
        {
            weekProgress.Set(finisedBreathings);
            return UniTask.CompletedTask;
        }

        public UniTask InitializeTimer()
        {
            timer.Initialize();
            return UniTask.CompletedTask;
        }
        
        public async UniTask ActualizeWeekCalendar(DayOfWeek dayOfWeek, IReadOnlyList<FinishedBreathing> finishedBreathings)
        {
            await weekProgress.Actualize(dayOfWeek, finishedBreathings);
        }
        
        protected override void OnDeInit()
        {
            base.OnDeInit();
            breathingButtons?.ForEach(button => menuButton.ReleaseInstance(button));
        }
    }
}