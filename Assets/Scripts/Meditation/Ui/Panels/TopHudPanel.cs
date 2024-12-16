using Cysharp.Threading.Tasks;
using Meditation.Ui.Components;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace Meditation.Ui.Panels
{
    public class TopHudPanel : UiPanel
    {
        [SerializeField] private TotalBreathCounter totalBreathCounter;
        [SerializeField] private StreakCounter streakCounter;
        
        public override UniTask Initialize()
        {
            totalBreathCounter.Initialize();
            streakCounter.Initialize();
            return UniTask.CompletedTask;
        }
    }
}