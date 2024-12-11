using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Reporting;

namespace OneDay.Core.Editor.Build
{
    public class BuildAutomation : MonoBehaviour
    {
        [MenuItem("Build/Build APK")]
        public static void BuildAndroidAPK()
        {
            var buildSummary = AndroidBuilder.Build(LoadBuildSettings(), false);
            ShowBuildSummary(buildSummary, null);
        }

        [MenuItem("Build/Build AAB")]
        public static void BuildAndroidAAB()
        {
            var buildSummary = AndroidBuilder.Build(LoadBuildSettings(), true);
            ShowBuildSummary(buildSummary, null);
        }
        
        [MenuItem("Build/Build AAB and Upload")]
        public static void BuildAndroidAABAndUpload()
        {
            var buildSettings = LoadBuildSettings();
            var buildSummary = AndroidBuilder.Build(buildSettings, true);
            UploadSummary uploadSummary = null;
            if (buildSummary.result == BuildResult.Succeeded)
            {
                uploadSummary = UploadABBToGoogleConsole.Upload(
                    buildSettings.pathToPython,
                    Path.Combine(Application.dataPath, buildSettings.googleUploadScriptPath),
                    buildSummary.outputPath,
                    Path.Combine(Application.dataPath, buildSettings.serviceAccountJsonFile),
                    PlayerSettings.applicationIdentifier,
                    PlayerSettings.Android.bundleVersionCode);
            }

            ShowBuildSummary(buildSummary, uploadSummary);
        }

        [MenuItem("Build/Test Upload")]
        public static void TestUpload()
        {
            var buildSettings = LoadBuildSettings();
            UploadABBToGoogleConsole.Upload(
                buildSettings.pathToPython,
                Path.Combine(Application.dataPath, buildSettings.googleUploadScriptPath),
                "testPath",
                Path.Combine(Application.dataPath,buildSettings.serviceAccountJsonFile),
                PlayerSettings.applicationIdentifier,
                PlayerSettings.Android.bundleVersionCode);
        }

       
        internal static string[] GetScenes()
        {
            // Fetch all enabled scenes in the Build Settings
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
        }

        private static BuildSettings LoadBuildSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<BuildSettings>("Assets/Editor/BuildSettings.asset");
            if (settings == null)
            {
                Debug.LogError("BuildSettings.asset not found. Please create one in the Assets/Editor folder.");
            }

            return settings;
        }

        private static void ShowBuildSummary(BuildSummary summary, UploadSummary uploadSummary)
        {
            string result = $"Build was {summary.result}";
            result += $"\nErrors: {summary.totalErrors}";

            if (uploadSummary != null)
            {
                result += $"\n\nUpload was {(uploadSummary.IsSuccessful ? "Successful" : "Failed")}";
                result += $"\n {uploadSummary.Output}";
            }

            EditorUtility.DisplayDialog("Build result", result, "OK");
        }
    }
}