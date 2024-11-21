using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Ui;
using Meditation.Ui.Components;
using UnityEngine;

namespace Meditation
{
    public interface IUiManager
    {
        UniTask PostInitialize();
        UniTask HideRootView(bool smooth = true);
        UniTask ShowRootViews(bool smooth = true);
        T GetView<T>() where T : UiView;
        IEnumerable<UiView> GetAllViews();
        T GetPopup<T>() where T : UiPopup;
        IEnumerable<UiPopup> GetAllPopups();
        PopupRequest<T> OpenPopup<T>(IUiParameter parameter) where T : UiPopup;
    }
   
    
    public class PopupRequest<T> where T: UiPopup
    {
        public UniTask OpenTask { get; set; }
        public T Popup { get; set; }

        public async UniTask WaitForClose() =>
            await UniTask.WaitUntil(()=>Popup.State == UiPopup.PopupState.Closed);
    }
    
    public class UiManager: MonoBehaviour, IService, IUiManager
    {
        [SerializeField] private List<UiView> views;
        [SerializeField] private List<UiPopup> popups;
        
        [SerializeField] private CanvasGroup sharedViewCg;
        [SerializeField] private SettingsPopup settingsPopup;
        [SerializeField] private TotalBreathCounter totalBreathCounter;
        [SerializeField] private StreakCounter streakCounter;

        public UniTask Initialize()
        {
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
        public PopupRequest<T> OpenPopup<T>(IUiParameter parameter) where T: UiPopup
        {
            var popup = GetPopup<T>();
            
            var request = new PopupRequest<T>
            {
                Popup = GetPopup<T>(),
                OpenTask = popup.Open(parameter)
            };
            return request;
        }
        
        public T GetPopup<T>() where T : UiPopup => (T)popups.FirstOrDefault(x => x.GetType() == typeof(T));
        public IEnumerable<UiPopup> GetAllPopups() => popups;
        #endregion
      
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