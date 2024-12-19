using Cysharp.Threading.Tasks;
using DG.Tweening;
using OneDay.Core;
using UnityEngine;

namespace Meditation.Apis.Visual
{
    public interface IVisualEnvironmentManager
    {
        UniTask FadeInSkybox();
    }
    
    public class VisualEnvironmentManager : MonoBehaviour, IVisualEnvironmentManager, IService
    {
        [SerializeField] private Material skyBoxMaterial;
        public UniTask Initialize() => UniTask.CompletedTask;
        public UniTask PostInitialize() => UniTask.CompletedTask;
      
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
    }
}