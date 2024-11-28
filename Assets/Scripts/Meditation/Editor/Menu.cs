using Meditation.Apis.Data;
using Meditation.Data;
using Meditation.Localization;
using OneDay.Core.Modules.Data;
using UnityEditor;

public class Menu
{
    [MenuItem("CalmWaves/Delete all data")]
    private static void DeleteAllData()
    {
        LocalStorage.RemoveAllEditor(
            TypeToDataKeyBinding.User, 
            TypeToDataKeyBinding.FinishedBreathing,
            TypeToDataKeyBinding.BreathingTestResult,
            TypeToDataKeyBinding.CustomBreathingSettings); 
    }
    
    [MenuItem("CalmWaves/Dump all data")]
    private static void DumpAllData()
    {
        LocalStorage.Dump<FinishedBreathing>(TypeToDataKeyBinding.User); 
        LocalStorage.Dump<FinishedBreathing>(TypeToDataKeyBinding.FinishedBreathing); 
        LocalStorage.Dump<BreathingTestResult>(TypeToDataKeyBinding.BreathingTestResult); 
        LocalStorage.Dump<BreathingTestResult>(TypeToDataKeyBinding.CustomBreathingSettings); 
    }
    
    [MenuItem("CalmWaves/Generate TextIds")]
    private static void SaveTextIds()
    {
        var db = LocalizationFactory.Create();
        LocalizationFactory.GenerateClassWithConstants(db);
    }
}
