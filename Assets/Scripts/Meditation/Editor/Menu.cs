using Meditation;
using Meditation.Apis.Data;
using Meditation.Data;
using UnityEditor;

public class Menu
{
    [MenuItem("CalmWaves/Delete all data")]
    private static void DeleteAllData()
    {
        LocalStorage.RemoveAllEditor(
            TypeToDataKeyBinding.User, 
            TypeToDataKeyBinding.FinishedBreathing,
            TypeToDataKeyBinding.BreathingTestResult); 
    }
    
    [MenuItem("CalmWaves/Dump all data")]
    private static void DumpAllData()
    {
        LocalStorage.Dump<FinishedBreathing>(TypeToDataKeyBinding.User); 
        LocalStorage.Dump<FinishedBreathing>(TypeToDataKeyBinding.FinishedBreathing); 
        LocalStorage.Dump<BreathingTestResult>(TypeToDataKeyBinding.BreathingTestResult); 
    }
}
