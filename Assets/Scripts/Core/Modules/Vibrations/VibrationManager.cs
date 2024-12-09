using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Vibrations
{
    public interface IVibrationManager
    {
        void VibrateTiny();
        void VibratePeek();
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
        public UniTask PostInitialize() => UniTask.CompletedTask;
        public void VibrateTiny() => Vibration.VibratePop();
        public void VibratePeek() => Vibration.VibratePeek();

#if UNITY_ANDROID
        public void VibrateCustom(int ms) => Vibration.VibrateAndroid(ms);
#endif
    }
}