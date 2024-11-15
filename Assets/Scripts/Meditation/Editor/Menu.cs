using Meditation;
using Meditation.Apis.Data;
using UnityEditor;

public class Menu
{
    [MenuItem("CalmWaves/Delete all data")]
    private static void DeleteAllData()
    {
        LocalStorage.RemoveAllEditor<FinishedBreathing>(); 
    }
    
    [MenuItem("CalmWaves/Dump all data")]
    private static void DumpAllData()
    {
        LocalStorage.Dump<FinishedBreathing>(); 
    }
}
