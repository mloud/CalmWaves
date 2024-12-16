using System;
using System.Collections.Generic;
using Core.Modules.Ui.Effects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Ui.Calendar;
using Meditation.Ui.Chart;
using Meditation.Ui.Components;
using OneDay.Core;
using OneDay.Core.Modules.Conditions.Ui;
using OneDay.Core.Modules.Store;
using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class MenuView : UiView
    {
        public Button StartButton => startButton;
        public Button AiButton => aiButton;
        public Button MeasuringButton => measuringButton;
        public Button NotificationButton => notificationButton;
        public Button SubscriptionButton => subscriptionButton;
        public Button MusicButton => musicButton;
        public DayTimeSpanChart BreathingChart => breathingChart;
        public CustomExerciseContainer CustomExerciseContainer => customExerciseContainer;
        public List<ConditionComponent> ConditionComponents => conditionComponents;
        
        [Header("Buttons")]
        [SerializeField] private Button sleepButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button aiButton;
        [SerializeField] private Button measuringButton;
        [SerializeField] private Button notificationButton;
        [SerializeField] private Button subscriptionButton;
        [SerializeField] private Button musicButton;

        
        [SerializeField] private UiElement topPagePart;
        [SerializeField] private Material skyBoxMaterial;
        [SerializeField] private Transform container;
        [SerializeField] private AssetReferenceGameObject menuButton;
        
        [SerializeField] private WeekProgress weekProgress;
        [SerializeField] private DayTimeSpanChart breathingChart;
        [SerializeField] private Timer.Timer timer;
        [SerializeField] private CustomExerciseContainer customExerciseContainer;
        [SerializeField] private ScrollRect mainScrollRect;
        [SerializeField] private ExpandableArea expandableArea;
        [SerializeField] private List<ConditionComponent> conditionComponents;
        
        private List<GameObject> breathingButtons;

        protected override void OnInit()
        {
            base.OnInit();
            expandableArea.OnExpanded += (expanded) =>
            {
                if (expanded)
                    topPagePart.Hide(true).Forget();
                else
                    topPagePart.Show(true).Forget();
            };
        }
        
        public async UniTask InitializeBreathingButtons(IEnumerable<IBreathingSettings> settings, Action<string> menuButtonClicked)
        {
            breathingButtons = new List<GameObject>();
            foreach (var setting in settings)
            {
                var button = await menuButton.InstantiateAsync(container);
                breathingButtons.Add(button);
                button.GetComponent<BreathingButton>().Set(setting, setting.GetName(), menuButtonClicked).Forget();
            }
        }

        public override async UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            mainScrollRect.verticalNormalizedPosition = 1;
            await UniTask.WhenAll(conditionComponents.Select(x => x.Refresh()));
            await base.Show(useSmooth, speedMultiplier);
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

        public UniTask InitializePremium()
        {
            ServiceLocator.Get<IStoreManager>().ProductPurchased += _ => 
                conditionComponents.ForEach(x => x.Refresh());
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