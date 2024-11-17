using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.States;
using Meditation.Visualizers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class BreathingView : UiView
    {
        [SerializeField] private CanvasGroup elementsCg;
        
        public ATotalTimeVisualizer TotalTimeVisualizer=> totalTimeVisualizer;
        public ACountDownVisualizer CountDownVisualizer=> countDownVisualizer;
        public ABreathingVisualizer BreathingVisualizer=> breathingVisualizer;
        public ABreathStatisticVisualizer BreathStatisticVisualizer=> breathStatisticVisualizer;
        public TextFader PauseText => pauseText;
        public TextFader TextFader => textFader;
        public TextMeshProUGUI NameLabel => nameLabel;
        public Button PauseButton => pauseButton;
        public Button SettingsButton => settingsButton;
        
        [SerializeField] private ATotalTimeVisualizer totalTimeVisualizer;
        [SerializeField] private ACountDownVisualizer countDownVisualizer;
        [SerializeField] private ABreathingVisualizer breathingVisualizer;
        [SerializeField] private ABreathStatisticVisualizer breathStatisticVisualizer;
        [SerializeField] private TextFader textFader;
        [SerializeField] private TextFader pauseText;
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button settingsButton;
        protected override void OnInit()
        {
            base.OnInit();
            LookUp.Get<BreathingView>().Register(this);
        }
   
        public async UniTask FadeOutElements()
        {
            await elementsCg.DOFade(0, 1.0f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }
        
        public async UniTask FadeInElements()
        {
            await elementsCg.DOFade(1, 1.0f)
                .From(0)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }

        public UniTask HideElements()
        {
            elementsCg.alpha = 0;
            return UniTask.CompletedTask;
        }
    }
}