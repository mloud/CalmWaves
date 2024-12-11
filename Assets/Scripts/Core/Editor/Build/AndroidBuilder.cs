using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace OneDay.Core.Editor.Build
{
    public static class AndroidBuilder
    {
        public static BuildSummary Build(BuildSettings buildSettings, bool appBundle)
        {
            if (!Directory.Exists(buildSettings.outputPath))
            {
                Directory.CreateDirectory(buildSettings.outputPath);
            }

            var options = new BuildPlayerOptions
            {
                scenes = BuildAutomation.GetScenes(), // Get all enabled scenes
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            if (buildSettings.cleanBuild)
                options.options |= BuildOptions.CleanBuildCache;
            if (buildSettings.developmentBuild)
                options.options |= BuildOptions.Development;
           
            PlayerSettings.SetScriptingBackend(
                BuildTargetGroup.Android, appBundle 
                    ? ScriptingImplementation.IL2CPP 
                    : ScriptingImplementation.Mono2x);

            buildSettings.IncrementVersionCode();
            //BUILD
            var buildFileName = $"{buildSettings.buildName}_({buildSettings.bundleVersionCode}).{(EditorUserBuildSettings.buildAppBundle? "aab":"apk")}";
            var buildFullLocation = Path.Combine(buildSettings.outputPath, buildFileName);
            options.locationPathName = buildFullLocation;
            Debug.Log($"Building {buildFullLocation}");
            PlayerSettings.Android.buildApkPerCpuArchitecture = true;
            PlayerSettings.Android.bundleVersionCode = buildSettings.bundleVersionCode;
            EditorUserBuildSettings.buildAppBundle = appBundle;
            var report = BuildPipeline.BuildPlayer(options);
        
            Debug.Log($"Build completed! Files saved to {buildSettings.outputPath}");
            return report.summary;
        }

    }
}