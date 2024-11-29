using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace Meditation.Ui
{
    public class SettingsPopup : UiPopup
    {
        [SerializeField] private MusicSection musicSection;
        [SerializeField] private SoundSection soundSection;

        protected override UniTask OnOpenStarted(IUiParameter parameter)
        {
            ServiceLocator.Get<IUiManager>().HideView();
            musicSection.Initialize();
            soundSection.Initialize();
            return UniTask.CompletedTask;
        }
        
        protected override UniTask OnCloseStarted()
        {
            ServiceLocator.Get<IUiManager>().ShowView();
            return UniTask.CompletedTask;
        }
    }
}