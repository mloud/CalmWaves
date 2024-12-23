using Cysharp.Threading.Tasks;
using DG.Tweening;
using OneDay.Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace OneDay.Core.Modules.Performance
{
    public interface IPerformanceManager
    {
        void SetScreenDimmed(bool isDimmed);
        void SwitchToLowPerformance();
        void SwitchToHighPerformance();
    }
    
    public class PerformanceManager : MonoBehaviour, IService, IPerformanceManager
    {
        [SerializeField] private Image dimLayer;

        public UniTask Initialize()
        {
            dimLayer.SetAlpha(0);
            return UniTask.CompletedTask;   
        }
        public UniTask PostInitialize() => UniTask.CompletedTask;
       
        public void SetScreenDimmed(bool isDimmed)
        {
            dimLayer.DOFade(isDimmed ? 0.98f : 0, 1.0f);
        }

        public void SwitchToLowPerformance()
        {
            Application.targetFrameRate = 15;
        }

        public void SwitchToHighPerformance()
        {
            Application.targetFrameRate = 60;
        }
    }
}