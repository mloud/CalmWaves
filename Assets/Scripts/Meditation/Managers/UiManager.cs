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
        T GetPopup<T>() where T : UiPopup;
        IEnumerable<UiPopup> GetAllPopups();
        PopupRequest OpenPopup<T>(IUiParameter parameter) where T : UiPopup;
    }
   
    
    public class PopupRequest
    {
        public UniTask OpenTask { get; set; }
        public UiPopup Popup { get; set; }

        public async UniTask WaitForClose() =>
            await UniTask.WaitUntil(()=>Popup.State == UiPopup.PopupState.Closed);
    }
    
    public class UiManager: MonoBehaviour, IService, IUiManager
    {
        [SerializeField] private List<UiView> views;
        [SerializeField] private List<UiPopup> popups;
        
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
            GetAllPopups().ForEach(x=>x.Hide(false));
            return UniTask.CompletedTask;
        }

        public async UniTask PostInitialize()
        {
            totalBreathCounter.Initialize();
            streakCounter.Initialize();
            await HideRootView(false);
        }


        #region Views
        public T GetView<T>() where T : UiView => (T)views.FirstOrDefault(x => x.GetType() == typeof(T));
        public IEnumerable<UiView> GetAllViews() => views;
        #endregion

        #region Popups
        public PopupRequest OpenPopup<T>(IUiParameter parameter) where T: UiPopup
        {
            var popup = GetPopup<T>();
            
            var request = new PopupRequest
            {
                Popup = GetPopup<T>(),
                OpenTask = popup.Open(parameter)
            };
            return request;
        }
        
        public T GetPopup<T>() where T : UiPopup => (T)popups.FirstOrDefault(x => x.GetType() == typeof(T));
        public IEnumerable<UiPopup> GetAllPopups() => popups;
        #endregion
        
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