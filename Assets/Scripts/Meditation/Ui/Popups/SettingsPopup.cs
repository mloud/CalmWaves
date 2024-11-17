using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class SettingsPopup : UiElement
    {
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private Button backButton;
        [SerializeField] private MusicSection musicSection;
        [SerializeField] private SoundSection soundSection;

        private void Awake()
        {
            backButton.onClick.AddListener(OnBack);
        }

        public async UniTask Show()
        {
            musicSection.Initialize();
            soundSection.Initialize();
            
            gameObject.SetActive(true);
            await DOTween.To(() => cg.alpha, v => cg.alpha = v, 1.0f, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
        }
        
        public async UniTask Hide()
        {
            await DOTween.To(() => cg.alpha, v => cg.alpha = v, 0.0f, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }
        
        private void OnBack()
        {
            ServiceLocator.Get<IUiManager>().HideSettingsPopup();
        }
    }
}