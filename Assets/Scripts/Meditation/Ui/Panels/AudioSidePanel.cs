using Cysharp.Threading.Tasks;
using Meditation.Ui.Audio;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Panels
{
    public class AudioSidePanel : UiPanel
    {
        [SerializeField] private Button musicButton;
        
        public override UniTask Initialize()
        {
            musicButton.onClick.AddListener(()=>OnMusicClicked());
            return UniTask.CompletedTask;
        }
        
        private async UniTask OnMusicClicked()
        {
            var request = ServiceLocator.Get<IUiManager>().OpenPopup<AudioPopup>(null);
            await request.OpenTask;
        }
    }
}