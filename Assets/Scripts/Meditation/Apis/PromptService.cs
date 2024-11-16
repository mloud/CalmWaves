using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Meditation.Apis
{
    public interface IPromptService
    {
        UniTask<string> SendRequest(string prompt);
    }

    public class PromptService : IPromptService
    {
        private string url;
        
        public PromptService(string url)
        {
            this.url = url;
        }
        
        public async UniTask<string> SendRequest(string prompt)
        {
            // Log the request prompt for debugging
            Debug.Log($"Requesting prompt: {prompt}");

            var form = new WWWForm();
            // Add the parameter to the form that matches the expected input of the Google Apps Script
            form.AddField("parameter", prompt);

            // Create a POST request to the API
            using var request = UnityWebRequest.Post(url, form);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            await request.SendWebRequest().ToUniTask();
 
            // Handle the result of the web request
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    // Log connection errors
                    Debug.LogError("Connection error: " + request.error);
                    return null;
                case UnityWebRequest.Result.ProtocolError:
                    // Log protocol errors (e.g., 404, 500)
                    Debug.LogError("Protocol error: " + request.error);
                    return null;
                default:
                    // Log the successful response and pass it to the callback
                    Debug.Log("Response: " + request.downloadHandler.text);
                    return request.downloadHandler.text;
            }
        }
    }
}