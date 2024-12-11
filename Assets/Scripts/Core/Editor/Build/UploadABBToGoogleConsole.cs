using System;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace OneDay.Core.Editor.Build
{
    public class UploadABBToGoogleConsole : MonoBehaviour
    {
        public static UploadSummary Upload(
            string pythonPath, 
            string pythonScriptPath, 
            string aabPath,
            string serviceAccountJsonFilePath, 
            string bundleId, 
            int versionCode)
        {
            PrintPythonVersion();
   
            if (!File.Exists(pythonScriptPath))
            {
                return new UploadSummary(false, $"Python script not found at {pythonScriptPath}");
            }

            if (!File.Exists(serviceAccountJsonFilePath))
            {
                return new UploadSummary(false, "Google service json not found at {serviceAccountJsonFilePath}");
            }
            
             // Build the arguments string
            var args = $"--aab_file {aabPath} --service_account_json_file \"{serviceAccountJsonFilePath}\" --bundle_id {bundleId} --version_code {versionCode}";


            // Configure the process
            var startInfo = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"{pythonScriptPath} {args}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            // Start the process
            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return new UploadSummary(false,"Failed to start the Python Upload process.");
            }
         
            // Capture output and error
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            
            process.WaitForExit();

            // Log output and errors
            if (output.Contains("error", StringComparison.CurrentCultureIgnoreCase))
            {
                return new UploadSummary(false, $"Python Script Error:\n{output}");
            }
            // Log output and errors
            if (!string.IsNullOrEmpty(error))
            {
                return new UploadSummary(false, $"Python Script Error:\n{error}");
            }

            return new UploadSummary(true, output);
        }
        
        private static void PrintPythonVersion()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = "--version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            
            process.WaitForExit();

            Debug.Log($"Python Version: {output}");
            Debug.Log($"Python Path: {startInfo.FileName}");
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Error: {error}");
            }
        }
    }
}