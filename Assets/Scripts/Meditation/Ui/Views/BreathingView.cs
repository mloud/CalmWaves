using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Visualizers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class BreathingView : UiView
    {
        [SerializeField] private CanvasGroup elementsCg;
        [SerializeField] private CanvasGroup basicElementsCg;
        public ATotalTimeVisualizer TotalTimeVisualizer=> totalTimeVisualizer;
        public ACountDownVisualizer CountDownVisualizer=> countDownVisualizer;
        public ABreathingVisualizer BreathingVisualizer=> breathingVisualizer;
        public ABreathStatisticVisualizer BreathStatisticVisualizer=> breathStatisticVisualizer;
        public TextFader PauseText => pauseText;
        public TextMeshProUGUI NameLabel => nameLabel;
        public Button PauseButton => pauseButton;
        public Button SettingsButton => settingsButton;
        
        [SerializeField] private ATotalTimeVisualizer totalTimeVisualizer;
        [SerializeField] private ACountDownVisualizer countDownVisualizer;
        [SerializeField] private ABreathingVisualizer breathingVisualizer;
        [SerializeField] private ABreathStatisticVisualizer breathStatisticVisualizer;
        [SerializeField] private TextFader pauseText;
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button settingsButton;
     
        public async UniTask FadeOutElements(bool includeBasicElements = false)
        {
            var tasks = new List<UniTask>();
            if (includeBasicElements)
            {
                tasks.Add(basicElementsCg.DOFade(0, 1.0f)
                    .SetEase(Ease.Linear)
                    .ToUniTask());
            }

            tasks.Add(elementsCg.DOFade(0, 1.0f)
                .SetEase(Ease.Linear)
                .ToUniTask());

            await UniTask.WhenAll(tasks);
        }

        public async UniTask FadeInElements()
        {
            var tasks = new List<UniTask>
            {
                basicElementsCg.DOFade(1, 1.0f)
                    .SetEase(Ease.Linear)
                    .ToUniTask(),
                elementsCg.DOFade(1, 1.0f)
                    .SetEase(Ease.Linear)
                    .ToUniTask()
            };

            await UniTask.WhenAll(tasks);
        }

        public UniTask HideElements()
        {
            elementsCg.alpha = 0;
            return UniTask.CompletedTask;
        }
    }
}