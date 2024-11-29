using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Vibrations
{
    public interface IVibrationManager
    {
        void VibrateTiny();
#if UNITY_ANDROID
        void VibrateCustom(int ms);
#endif
    }
    public class VibrationManager : MonoBehaviour, IVibrationManager, IService
    {
        public UniTask Initialize()
        {
            Vibration.Init();
            return UniTask.CompletedTask;
        }
        public void VibrateTiny()
        {
            Vibration.VibratePop();
        }

#if UNITY_ANDROID
        public void VibrateCustom(int ms)
        {
            Vibration.VibrateAndroid(ms);
        }
#endif
    }
}