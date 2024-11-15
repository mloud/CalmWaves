using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Ui;
using Meditation.Ui.Views;
using UnityEngine;

namespace Meditation
{
    public interface IUiManager
    {
        UniTask ShowInfoPopup(IBreathingSettings breathingSettings, bool hideView, bool hasCloseButton);
        UniTask HideInfoPopup();
    }
    
    public class UiManager: MonoBehaviour, IService, IUiManager
    {
        [SerializeField] private CanvasGroup sharedViewCg;
        [SerializeField] private Popup infoPopup;
        [SerializeField] private BreathingView breathingView;
        [SerializeField] private MenuView menuView;

        public UniTask Initialize()
        {
            infoPopup.gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
        
        public async UniTask ShowInfoPopup(IBreathingSettings breathingSettings, bool hideView, bool hasCloseButton)
        {
            var tasks = new List<UniTask>();
            if (hideView)
            {
                tasks.Add(HideAllViews());
            }
            tasks.Add( infoPopup.Show(breathingSettings, hasCloseButton));
            await UniTask.WhenAll(tasks);
        }

        public async UniTask HideInfoPopup()
        {
            await UniTask.WhenAll(
                infoPopup.Hide(),
                ShowAllViews());
        }

        private async UniTask HideAllViews()
        {
            await sharedViewCg.DOFade(0, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }
        
        private async UniTask ShowAllViews()
        {
            await sharedViewCg.DOFade(1, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }
    }
}