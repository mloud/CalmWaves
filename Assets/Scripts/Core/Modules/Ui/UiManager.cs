using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Ui;
using OneDay.Core.Extensions;
using UnityEngine;

namespace OneDay.Core.Modules.Ui
{
    public interface IUiManager
    {
        UniTask HideView(bool smooth = true);
        UniTask ShowView(bool smooth = true);
        
        T GetView<T>() where T : UiView;
        IEnumerable<UiView> GetAllViews();
        T GetPopup<T>() where T : UiPopup;
        IEnumerable<UiPopup> GetAllPopups();
        
        T GetPanel<T>() where T : UiPanel;
        IEnumerable<UiPanel> GetAllPanels();
        PopupRequest<T> OpenPopup<T>(IUiParameter parameter) where T : UiPopup;
    }
   
    
    public class PopupRequest<T> where T: UiPopup
    {
        public UniTask OpenTask { get; set; }
        public T Popup { get; set; }

        public async UniTask WaitForCloseStarted() =>
            await UniTask.WaitUntil(()=>Popup.State == UiPopup.PopupState.Closing);
        public async UniTask WaitForCloseFinished() =>
            await UniTask.WaitUntil(()=>Popup.State == UiPopup.PopupState.Closed);
    }
    
    public class UiManager: MonoBehaviour, IService, IUiManager
    {
        [SerializeField] private List<UiView> views;
        [SerializeField] private List<UiPopup> popups;
        [SerializeField] private List<UiPanel> panels;
        [SerializeField] private CanvasGroup sharedViewCg;
  
        public UniTask Initialize()
        {
            GetAllPopups().ForEach(x=>x.Hide(false));
            GetAllViews().ForEach(x=>x.Hide(false));
            
            GetAllPopups().ForEach(x=>x.Initialize());
            return UniTask.CompletedTask;
        }

        public UniTask PostInitialize()
        {
            GetAllPopups().ForEach(x=>x.Initialize());
            return UniTask.CompletedTask;
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
        
        public T GetPanel<T>() where T : UiPanel => (T)panels.FirstOrDefault(x => x.GetType() == typeof(T));
        public IEnumerable<UiPanel> GetAllPanels() => panels;
      
        public async UniTask HideView(bool smooth = true)
        {
            if (smooth)
            {
                await sharedViewCg.DOFade(0, 0.5f)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }
            sharedViewCg.gameObject.SetActive(false);
        }
        
        public async UniTask ShowView(bool smooth = true)
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