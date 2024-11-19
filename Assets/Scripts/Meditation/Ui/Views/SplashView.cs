using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Meditation.Ui.Views
{
    public class SplashView : UiView
    {
        [SerializeField] private float duration;
        protected override void OnInit()
        {
            //base.OnInit();
            LookUp.Get<SplashView>().Register(this);
        }

        public async UniTask Animate()
        {
            await UniTask.WaitForSeconds(duration);
        }
    }
}