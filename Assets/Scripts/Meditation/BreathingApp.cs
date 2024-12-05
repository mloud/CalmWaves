using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Data;
using Meditation.Apis.Measure;
using Meditation.Apis.Settings;
using Meditation.Data;
using Meditation.Data.Notifications;
using Meditation.Localization;
using Meditation.Managers;
using Meditation.States;
using Meditation.Ui.Panels;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Audio;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Localization;
using OneDay.Core.Modules.Notifications;
using OneDay.Core.Modules.Share;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Update;
using OneDay.Core.Modules.Vibrations;
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
        [SerializeField] private ShareManager shareManager;
        [SerializeField] private MeasureManager measureManager;
        [SerializeField] private LocalizationManager localizationManager;
        [SerializeField] private VibrationManager vibrationManager;
        [SerializeField] private NotificationManager notificationManager;
        [SerializeField] private NotificationsApi notificationsApi; 
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
            ServiceLocator.Register<IMeasure>(measureManager);
            ServiceLocator.Register<ILocalizationManager>(localizationManager);
            ServiceLocator.Register<IVibrationManager>(vibrationManager);
            ServiceLocator.Register<INotificationManager>(notificationManager);
            ServiceLocator.Register<INotificationsApi>(notificationsApi);
            
            // Savable data
            ServiceLocator.Get<IDataManager>().RegisterStorage<FinishedBreathing>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<User>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<BreathingTestResult>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<CustomBreathingSettings>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<UserDayTimeNotificationSettings>(new LocalStorage());

            // Readonly data 
            ServiceLocator.Get<IDataManager>().RegisterStorage<ContentNotificationSettings>(new AddressableScriptableObjectStorage());


            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<FinishedBreathing>(TypeToDataKeyBinding.UserFinishedBreathing);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<User>(TypeToDataKeyBinding.UserData);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<BreathingTestResult>(TypeToDataKeyBinding.UserBreathingTestResult);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<CustomBreathingSettings>(TypeToDataKeyBinding.UserCustomBreathingSettings);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<ContentNotificationSettings>(TypeToDataKeyBinding.ContentNotificationSettings);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<UserDayTimeNotificationSettings>(TypeToDataKeyBinding.UserNotificationSettings);

            // load texts
            ServiceLocator.Get<ILocalizationManager>().LocalizationDatabase = LocalizationFactory.Create();
            ServiceLocator.Get<ILocalizationManager>().Language = "en";
            //  create default user
            if (!(await ServiceLocator.Get<IDataManager>().GetAll<User>()).Any())
            {
                ServiceLocator.Get<IDataManager>().Add(new User());
            }
            
            ServiceLocator.Register<IBreathingApi>(new BreathingApi());
            ServiceLocator.ForEach(x => x.Initialize());
           
            ServiceLocator.Get<ISettingsApi>().RegisterModule<IVolumeModule>(new VolumeModule());
            ServiceLocator.Get<ISettingsApi>().RegisterModule<ISoundSettingsModule>(new SettingsModule());
            await ServiceLocator.Get<IUiManager>().HideView(false);
            await ServiceLocator.Get<IUiManager>().GetPanel<TopHudPanel>().Initialize();
          
            
            
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