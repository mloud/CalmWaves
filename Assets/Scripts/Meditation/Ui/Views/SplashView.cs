using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Meditation.Ui.Views
{
    public class SplashView : UiView
    {
        [SerializeField] private TextMeshProUGUI appName;
        [SerializeField] private CanvasGroup bgCanvasGroup;
        protected override void OnInit()
        {
            //base.OnInit();
            LookUp.Get<SplashView>().Register(this);
        }

        #if UNITY_EDITOR
        public UniTask Animate(Action transitionAction)
        {
            transitionAction?.Invoke(); 
            Hide(false).Forget();
            return UniTask.CompletedTask;
        }
        #else
        public async UniTask Animate(Action transitionAction)
        {
            await appName.DOFade(1, 1.0f)
                .SetEase(Ease.Linear)
                .From(0)
                .AsyncWaitForCompletion();

            await UniTask.WaitForSeconds(2.0f);
            transitionAction?.Invoke();
            var appNameDefaultLabel = LookUp.Get<UiElement>().GetFirstWihName("AppNameLabel");

            appNameDefaultLabel.gameObject.SetActive(false);
            
            await UniTask.WhenAll(
                bgCanvasGroup.DOFade(0,1.0f).SetEase(Ease.Linear).ToUniTask(),
                appName.transform.DOMove(appNameDefaultLabel.transform.position, 1.0f).SetEase(Ease.Linear).ToUniTask(),
                appName.transform.DOScale(appNameDefaultLabel.transform.localScale, 1.0f).SetEase(Ease.Linear).ToUniTask());
            appNameDefaultLabel.gameObject.SetActive(true);
            await Hide(false);
        }
        #endif
    }
}