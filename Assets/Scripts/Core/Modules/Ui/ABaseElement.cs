using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Ui
{
    public abstract class ABaseElement : MonoBehaviour
    {
        public string Name => name;
        private void Awake()
        {
            OnInit();
        }

        private void OnDestroy()
        {
            OnDeInit();
        }
        
        public virtual UniTask Show(bool useSmooth, float speedMultiplier = 1.0f)
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public virtual UniTask Hide(bool useSmooth, float speedMultiplier = 1.0f)
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
        
        protected virtual void OnInit() {}

        protected virtual void OnDeInit() { }
    }
}