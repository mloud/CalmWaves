using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Managers.Messages.Data;
using OneDay.Core;
using OneDay.Core.Debugging;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace Meditation.Managers.Messages
{
    public interface IMessageManager
    {
        string GetNextMessage(string area);
    }
    
    [LogSection("Ui")]
    public class MessageManager : MonoBehaviour, IService, IMessageManager
    {
        private IDataManager dataManager;
        private Dictionary<string, List<string>> currentMessages;
        private Dictionary<string, List<string>> originalMessages;

        public async UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            await GetAllMessages();
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;
        
        public string GetNextMessage(string area)
        {
            if (currentMessages.ContainsKey(area))
            {
                if (currentMessages[area].Count > 0)
                {
                    int rndIndex = Random.Range(0, currentMessages[area].Count);
                    var message = currentMessages[area][rndIndex];
                    currentMessages[area].RemoveAt(rndIndex);
                    return message;
                }

                currentMessages = MakeCopy(originalMessages);
                return GetNextMessage(area);
            }

            D.LogError($"No messages area {area} exists", this);
            return null;
        }

        private async UniTask GetAllMessages()
        {
            var allMessages = await dataManager.GetAll<MessageDefinition>();
            
            originalMessages = allMessages
                .GroupBy(m => m.Area)
                .ToDictionary(g => 
                    g.Key, g => g.Select(m => m.Text).ToList());

            currentMessages = MakeCopy(originalMessages);
        }
      
        private static Dictionary<string, List<string>> MakeCopy( Dictionary<string, List<string>> source)
        {
            var copy = new Dictionary<string, List<string>>();
            foreach (var kvp in source)
            {
                copy[kvp.Key] = new List<string>(kvp.Value);
            }

            return copy;
        }
    }
}