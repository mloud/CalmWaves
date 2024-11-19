using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Ui;
using Meditation.Ui.Components;
using Meditation.Ui.Views;
using UnityEngine;

namespace Meditation
{
    public interface IUiManager
    {
        UniTask ShowInfoPopup(IBreathingSettings breathingSettings, bool hideView, bool hasCloseButton);
        UniTask ShowSettingsPopup(bool hideViews);
        UniTask HideInfoPopup();
        UniTask HideSettingsPopup();
        UniTask PostInitialize();
    }
    
    public class UiManager: MonoBehaviour, IService, IUiManager
    {
        [SerializeField] private CanvasGroup sharedViewCg;
        [SerializeField] private Popup infoPopup;
        [SerializeField] private SettingsPopup settingsPopup;
        [SerializeField] private BreathingView breathingView;
        [SerializeField] private MenuView menuView;
        [SerializeField] private TotalBreathCounter totalBreathCounter;
        public UniTask Initialize()
        {
            infoPopup.gameObject.SetActive(false);
            settingsPopup.gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize()
        {
            totalBreathCounter.Initialize();
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
        
        public async UniTask ShowSettingsPopup(bool hideViews)
        {
            var tasks = new List<UniTask>();
            if (hideViews)
            {
                tasks.Add(HideAllViews());
            }
            tasks.Add( settingsPopup.Show());
            await UniTask.WhenAll(tasks);
        }

        public async UniTask HideInfoPopup()
        {
            await UniTask.WhenAll(
                infoPopup.Hide(),
                ShowAllViews());
        }

        public async UniTask HideSettingsPopup()
        {
            await UniTask.WhenAll(
                settingsPopup.Hide(),
                ShowAllViews());
        }

        private async UniTask HideAllViews()
        {
            await sharedViewCg.DOFade(0, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
            sharedViewCg.gameObject.SetActive(false);
        }
        
        private async UniTask ShowAllViews()
        {
            sharedViewCg.gameObject.SetActive(true);
            await sharedViewCg.DOFade(1, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }
    }
}