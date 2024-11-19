using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        [SerializeField] private Material skyBoxMaterial;
        [SerializeField] private Transform container;
        [SerializeField] private AssetReferenceGameObject menuButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button aiButton;
        [SerializeField] private WeekProgress weekProgress;
        [SerializeField] private Timer.Timer timer;
        
        private List<GameObject> breathingButtons;

        protected override void OnInit()
        {
            base.OnInit();
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


        public async UniTask FadeInSkybox()
        {
            await skyBoxMaterial
                .DOFloat(1.0f, "_Exposure", 2.0f)
                .SetEase(Ease.Linear)
                .From(0)
                .AsyncWaitForCompletion();
        }

        private void OnValidate()
        {
            skyBoxMaterial.SetFloat("_Exposure",0);
        }

        public UniTask InitializeStartButton(Action menuButtonClicked)
        {
            startButton.onClick.AddListener(()=>menuButtonClicked());
            return UniTask.CompletedTask;
        }

        public UniTask InitializeWeekCalendar(
            IReadOnlyList<(DayOfWeek dayOfWeek, TimeSpan breathingDuration)> breathingTimesInWeek, TimeSpan requiredBreathingTime)
        {
            weekProgress.Set(breathingTimesInWeek,requiredBreathingTime);
            return UniTask.CompletedTask;
        }

        public UniTask InitializeTimer()
        {
            timer.Initialize();
            return UniTask.CompletedTask;
        }
        
        public async UniTask ActualizeWeekCalendar(DayOfWeek dayOfWeek, TimeSpan breathingTimeToday, TimeSpan breathingTimeRequired)
        {
            await weekProgress.Actualize(dayOfWeek, breathingTimeToday, breathingTimeRequired);
        }
        
        protected override void OnDeInit()
        {
            base.OnDeInit();
            breathingButtons?.ForEach(button => menuButton.ReleaseInstance(button));
        }
    }
}