using System.Linq;
using Core.Modules.Ui.Effects;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Apis.Audio;
using Meditation.Apis.Data;
using Meditation.Apis.Measure;
using Meditation.Apis.Settings;
using Meditation.Apis.Visual;
using Meditation.Data;
using Meditation.Data.Notifications;
using Meditation.Localization;
using Meditation.Managers;
using Meditation.Managers.Messages;
using Meditation.Managers.Messages.Data;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Audio;
using OneDay.Core.Modules.Conditions;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Localization;
using OneDay.Core.Modules.Notifications;
using OneDay.Core.Modules.Performance;
using OneDay.Core.Modules.Share;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Store;
using OneDay.Core.Modules.Store.Data;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Update;
using OneDay.Core.Modules.Vibrations;
using UnityEngine;


namespace Meditation
{
    [DefaultExecutionOrder(-100)]
    public class BreathingApp : MonoBehaviour
    {
        [SerializeField] private DebugSections debugSections;
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
        [SerializeField] private StoreManager storeManager;
        [SerializeField] private ConditionManager conditionManager;
        [SerializeField] private AudioEnvironmentManager audioEnvironmentManager;
        [SerializeField] private VisualEnvironmentManager visualEnvironmentManager;
        [SerializeField] private EffectManager effectManager;
        [SerializeField] private MessageManager messageManager;
        [SerializeField] private PerformanceManager performanceManager;

        private void Awake()
        {
            Boot().Forget();
        }

        private async UniTask Boot()
        {
            D.Initialize(debugSections.Sections);
            
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            await UnityGS.Initialize("production");
            
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
            ServiceLocator.Register<IStoreManager>(storeManager);
            ServiceLocator.Register<IConditionManager>(conditionManager);
            ServiceLocator.Register<IAudioEnvironmentManager>(audioEnvironmentManager);
            ServiceLocator.Register<IEffectManager>(effectManager);
            ServiceLocator.Register<IVisualEnvironmentManager>(visualEnvironmentManager);
            ServiceLocator.Register<IMessageManager>(messageManager);
            ServiceLocator.Register<IPerformanceManager>(performanceManager);

            // Savable data
            ServiceLocator.Get<IDataManager>().RegisterStorage<FinishedBreathing>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<User>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<BreathingTestResult>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<CustomBreathingSettings>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<UserDayTimeNotificationSettings>(new LocalStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<AudioMixSettings>(new LocalStorage());

            // Readonly data 
            ServiceLocator.Get<IDataManager>().RegisterStorage<ContentNotificationSettings>(new AddressableScriptableObjectStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<ProductSettings>(new AddressableScriptableObjectStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<AudioDefinition>(new AddressableScriptableObjectStorage());
            ServiceLocator.Get<IDataManager>().RegisterStorage<MessageDefinition>(new AddressableScriptableObjectStorage());

            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<FinishedBreathing>(TypeToDataKeyBinding.UserFinishedBreathing);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<User>(TypeToDataKeyBinding.UserData);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<BreathingTestResult>(TypeToDataKeyBinding.UserBreathingTestResult);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<CustomBreathingSettings>(TypeToDataKeyBinding.UserCustomBreathingSettings);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<ContentNotificationSettings>(TypeToDataKeyBinding.ContentNotificationSettings);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<ProductSettings>(TypeToDataKeyBinding.ContentStoreItemSettings);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<AudioMixSettings>(TypeToDataKeyBinding.AudioMixSettings);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<AudioDefinition>(TypeToDataKeyBinding.ContentAudioDefinition);
            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<MessageDefinition>(TypeToDataKeyBinding.ContentMessageDefinition);

            ServiceLocator.Get<IDataManager>().RegisterTypeToKeyBinding<UserDayTimeNotificationSettings>(TypeToDataKeyBinding.UserNotificationSettings);

            ServiceLocator.Get<IConditionManager>().RegisterCondition(ConditionIds.IsPremiumAccount,
                new CompositeOrCondition(
                    ()=>ServiceLocator.Get<IStoreManager>().IsSubscriptionActive("calmwaves.premium_weekly_subscription"),
                    ()=>ServiceLocator.Get<IStoreManager>().IsSubscriptionActive("calmwaves.premium_monthly_subscription"),
                    ()=>ServiceLocator.Get<IStoreManager>().IsSubscriptionActive("calmwaves.premium_yearly_subscription")));

            
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
            ServiceLocator.ForEach(x => x.PostInitialize());
           
            ServiceLocator.Get<ISettingsApi>().RegisterModule<IVolumeModule>(new VolumeModule());
            ServiceLocator.Get<ISettingsApi>().RegisterModule<ISoundSettingsModule>(new SettingsModule());
            
            ServiceLocator.Get<IPerformanceManager>().SwitchToHighPerformance();

            
            await ServiceLocator.Get<IUiManager>().HideView(false);
            
            
            StateMachineEnvironment.UnregisterAll();
            StateMachineEnvironment.RegisterStateMachine("Application", new StateMachine(), true);
            
            await StateMachineEnvironment.Default.RegisterState<BootState>();
            await StateMachineEnvironment.Default.RegisterState<MenuState>();
            await StateMachineEnvironment.Default.RegisterState<BreathingState>();
            await StateMachineEnvironment.Default.RegisterState<MoodState>();
            await StateMachineEnvironment.Default.RegisterState<MeasuringState>();
            await StateMachineEnvironment.Default.RegisterState<SleepState>();
            await StateMachineEnvironment.Default.RegisterState<IntroState>();
            await StateMachineEnvironment.Default.SetStateAsync<BootState>();
        }
    }
}