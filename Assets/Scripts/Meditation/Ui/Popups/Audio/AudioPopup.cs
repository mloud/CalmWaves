using Cysharp.Threading.Tasks;
using Meditation.Apis.Audio;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace Meditation.Ui.Audio
{
    public class AudioPopup : UiPopup
    {
        [SerializeField] private AudioComponent audioComponent;

        private IAudioEnvironmentManager audioEnvironmentManager;

        public override async UniTask Initialize()
        {
            audioEnvironmentManager = ServiceLocator.Get<IAudioEnvironmentManager>();
        }

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            Debug.Assert(audioEnvironmentManager.Settings != null);
            await audioComponent.Initialize(audioEnvironmentManager.Settings);
            ServiceLocator.Get<IUiManager>().HideView();
        }

        protected override async UniTask OnCloseStarted()
        {
            await audioEnvironmentManager.Save(audioEnvironmentManager.Settings);
            ServiceLocator.Get<IUiManager>().ShowView();
        }
    }
}