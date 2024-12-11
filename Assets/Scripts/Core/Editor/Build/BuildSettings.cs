using UnityEditor;
using UnityEngine;

namespace OneDay.Core.Editor.Build
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Build/Settings")]
    public class BuildSettings : ScriptableObject
    {
        [Header("General Settings")]
        public string outputPath = "Builds";
        public string buildName = "MyApp";
        public bool developmentBuild = false;
        public bool cleanBuild = true;
        
        [Header("Android Settings")]
        public int bundleVersionCode = 1;

        public string pathToPython = "/Library/Frameworks/Python.framework/Versions/3.13/bin/python3";
        public string googleUploadScriptPath = "Scripts/Core/Editor/Build/UploadToGoogle.py";
        public string serviceAccountJsonFile="Scripts/Core/Editor/Build/GoogleService.json";
        
        public void IncrementVersionCode()
        {
            bundleVersionCode++;
            EditorUtility.SetDirty(this); // Mark the asset as dirty so the change is saved
            AssetDatabase.SaveAssets();   // Save the updated version code to disk
            Debug.Log($"Bundle version code incremented to: {bundleVersionCode}");
        }
    }
}