using Meditation.Apis.Data;
using Meditation.Data;
using Meditation.Data.Notifications;
using Meditation.Localization;
using OneDay.Core.Modules.Data;
using UnityEditor;

public class Menu
{
    [MenuItem("CalmWaves/Delete all data")]
    private static void DeleteAllData()
    {
        LocalStorage.RemoveAllEditor(
            TypeToDataKeyBinding.UserData, 
            TypeToDataKeyBinding.UserFinishedBreathing,
            TypeToDataKeyBinding.UserBreathingTestResult,
            TypeToDataKeyBinding.UserCustomBreathingSettings); 
    }
    
    [MenuItem("CalmWaves/Dump all data")]
    private static void DumpAllData()
    {
        LocalStorage.Dump<FinishedBreathing>(TypeToDataKeyBinding.UserData); 
        LocalStorage.Dump<FinishedBreathing>(TypeToDataKeyBinding.UserFinishedBreathing); 
        LocalStorage.Dump<BreathingTestResult>(TypeToDataKeyBinding.UserBreathingTestResult); 
        LocalStorage.Dump<BreathingTestResult>(TypeToDataKeyBinding.UserCustomBreathingSettings); 
        LocalStorage.Dump<UserDayTimeNotificationSettings>(TypeToDataKeyBinding.UserNotificationSettings); 
    }
    
    [MenuItem("CalmWaves/Generate TextIds")]
    private static void SaveTextIds()
    {
        var db = LocalizationFactory.Create();
        LocalizationFactory.GenerateClassWithConstants(db);
    }
}
