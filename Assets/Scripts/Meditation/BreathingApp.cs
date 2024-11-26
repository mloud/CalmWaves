using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Data;
using Meditation.Apis.Measure;
using Meditation.Apis.Settings;
using Meditation.Data;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Share;
using OneDay.Core.Sm;
using OneDay.Core.Ui;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("shareApi")] [SerializeField] private ShareManager shareManager;
        [SerializeField] private MeasureApi measureApi;
        private void Awake()
        {
            Boot().Forget();
        }

        private async UniTask Boot()
        {
            Application.targetFrameRate = 60;
            ServiceLocator.Register<IAudioManager>(audioManager);
            ServiceLocator.Register<IAssetManager>(assetManager);
            ServiceLocator.Register<IUiManager>(uiManager);
            ServiceLocator.Register<IDataManager>(dataManager);
            ServiceLocator.Register<IBreathGeneratorApi>(smartBreathGenerator);
            ServiceLocator.Register<ISettingsApi>(settingsApi);
            ServiceLocator.Register<IUpdateManager>(updateManager);
            ServiceLocator.Register<IShare>(shareManager);
            ServiceLocator.Register<IMeasureApi>(measureApi);
          
            ServiceLocator.Get<IDataManager>().RegisterStorage<FinishedBreathing>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<User>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<BreathingTestResult>(new LocalStorage());

            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<FinishedBreathing>(TypeToDataKeyBinding.FinishedBreathing);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<User>(TypeToDataKeyBinding.User);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<BreathingTestResult>(TypeToDataKeyBinding.BreathingTestResult);

            //  create default user
            if (!(await ServiceLocator.Get<IDataManager>().GetAll<User>()).Any())
            {
                ServiceLocator.Get<IDataManager>().Add<User>(new User());
            }
            
            ServiceLocator.Register<IBreathingApi>(new BreathingApi());
            ServiceLocator.ForEach(x => x.Initialize());
           
            ServiceLocator.Get<ISettingsApi>().RegisterModule<IVolumeModule>(new VolumeModule());
            ServiceLocator.Get<ISettingsApi>().RegisterModule<ISoundSettingsModule>(new SettingsModule());

            await ServiceLocator.Get<IUiManager>().PostInitialize();
            
            StateMachineEnvironment.UnregisterAll();
            StateMachineEnvironment.RegisterStateMachine("Application", new StateMachine(), true);
            
            await StateMachineEnvironment.Default.RegisterState<BootState>();
            await StateMachineEnvironment.Default.RegisterState<MenuState>();
            await StateMachineEnvironment.Default.RegisterState<BreathingState>();
            await StateMachineEnvironment.Default.RegisterState<MoodState>();
            await StateMachineEnvironment.Default.RegisterState<MeasuringState>();
            await StateMachineEnvironment.Default.SetStateAsync<BootState>();
        }
    }
}