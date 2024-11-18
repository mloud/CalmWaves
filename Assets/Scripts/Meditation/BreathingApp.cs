using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Data;
using Meditation.Apis.Settings;
using Meditation.States;
using UnityEngine;

namespace Meditation
{
    public class BreathingApp : MonoBehaviour
    {
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private AssetManager assetManager;
        [SerializeField] private UiManager uiManager;
        [SerializeField] private DataManager dataManager;
        [SerializeField] private SmartBreathGeneratorApi smartBreathGenerator;
        [SerializeField] private SettingsApi settingsApi;
        [SerializeField] private UpdateManager updateManager;

        private void Awake()
        {
            Boot().Forget();
        }

        private async UniTask Boot()
        {
            ServiceLocator.Register<IAudioManager>(audioManager);
            ServiceLocator.Register<IAssetManager>(assetManager);
            ServiceLocator.Register<IUiManager>(uiManager);
            ServiceLocator.Register<IDataManager>(dataManager);
            ServiceLocator.Register<IBreathGeneratorApi>(smartBreathGenerator);
            ServiceLocator.Register<ISettingsApi>(settingsApi);
            ServiceLocator.Register<IUpdateManager>(updateManager);
          
            ServiceLocator.Get<IDataManager>().RegisterStorage<FinishedBreathing>(new LocalStorage());
            
            ServiceLocator.Register<IBreathingApi>(new BreathingApi());
            ServiceLocator.ForEach(x => x.Initialize());
           
            ServiceLocator.Get<ISettingsApi>().RegisterModule<IVolumeModule>(new VolumeModule());
            ServiceLocator.Get<ISettingsApi>().RegisterModule<ISoundSettingsModule>(new SettingsModule());

            
            StateMachineEnvironment.UnregisterAll();
            StateMachineEnvironment.RegisterStateMachine("Application", new StateMachine(), true);
            
            await StateMachineEnvironment.Default.RegisterState<BootState>();
            await StateMachineEnvironment.Default.RegisterState<MenuState>();
            await StateMachineEnvironment.Default.RegisterState<BreathingState>();
            await StateMachineEnvironment.Default.RegisterState<MoodState>();
            await StateMachineEnvironment.Default.SetStateAsync<BootState>();
        }
    }
}