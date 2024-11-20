using System.Collections.Generic;
using System.Linq;
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
        UniTask HideRootView(bool smooth = true);
        UniTask ShowRootViews(bool smooth = true);
        T GetView<T>() where T : UiView;
        IEnumerable<UiView> GetAllViews();
    }
    
    public class UiManager: MonoBehaviour, IService, IUiManager
    {
        [SerializeField] private List<UiView> views;
        
        [SerializeField] private CanvasGroup sharedViewCg;
        [SerializeField] private Popup infoPopup;
        [SerializeField] private SettingsPopup settingsPopup;
        [SerializeField] private BreathingView breathingView;
        [SerializeField] private MenuView menuView;
        [SerializeField] private TotalBreathCounter totalBreathCounter;
        [SerializeField] private StreakCounter streakCounter;

        public UniTask Initialize()
        {
            infoPopup.gameObject.SetActive(false);
            settingsPopup.gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        public async UniTask PostInitialize()
        {
            totalBreathCounter.Initialize();
            streakCounter.Initialize();
            await HideRootView(false);
        }

        public T GetView<T>() where T : UiView => (T)views.FirstOrDefault(x => x.GetType() == typeof(T));
        public IEnumerable<UiView> GetAllViews() => views;
        public async UniTask ShowInfoPopup(IBreathingSettings breathingSettings, bool hideView, bool hasCloseButton)
        {
            var tasks = new List<UniTask>();
            if (hideView)
            {
                tasks.Add(HideRootView());
            }
            tasks.Add( infoPopup.Show(breathingSettings, hasCloseButton));
            await UniTask.WhenAll(tasks);
        }
        
        public async UniTask ShowSettingsPopup(bool hideViews)
        {
            var tasks = new List<UniTask>();
            if (hideViews)
            {
                tasks.Add(HideRootView());
            }
            tasks.Add( settingsPopup.Show());
            await UniTask.WhenAll(tasks);
        }

        public async UniTask HideInfoPopup()
        {
            await UniTask.WhenAll(
                infoPopup.Hide(),
                ShowRootViews());
        }

        public async UniTask HideSettingsPopup()
        {
            await UniTask.WhenAll(
                settingsPopup.Hide(),
                ShowRootViews());
        }

        public async UniTask HideRootView(bool smooth = true)
        {
            if (smooth)
            {
                await sharedViewCg.DOFade(0, 0.5f)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }
            sharedViewCg.gameObject.SetActive(false);
        }
        
        public async UniTask ShowRootViews(bool smooth = true)
        {
            sharedViewCg.gameObject.SetActive(true);

            if (smooth)
            {
                await sharedViewCg.DOFade(1, 0.5f)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }

            sharedViewCg.alpha = 1.0f;
        }
    }
}