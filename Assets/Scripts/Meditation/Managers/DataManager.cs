using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Data;
using Newtonsoft.Json;
using UnityEngine;


namespace Meditation
{
    public interface IDataObject
    {
        int Id { get; set; }
    }

    public abstract class BaseDataObject : IDataObject
    {
        public int Id { get; set; }
    }
    
    public interface IDataManager : IStorage
    {
        void RegisterStorage<T>(IStorage storage);
        void UnregisterStorage<T>(IStorage storage);

        UniTask<AddressableAsset<BreathingSettingsDb>> GetBreathingSettings();
        UniTask<AddressableAsset<BreathingSettingsDb>> GetDailyBreathingSettings();
        UniTask<AddressableAsset<MoodDb>> GetMoodSettings();
    }
    
    public class DataManager :  MonoBehaviour, IService, IDataManager
    {
        private Dictionary<Type, IStorage> storages;

        public UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }

        public void RegisterStorage<T>(IStorage storage)
        {
            storages ??= new Dictionary<Type, IStorage>(); 
            storages.Add(typeof(T), storage);
        }

        public void UnregisterStorage<T>(IStorage storage)
        {
            storages.Remove(typeof(T));
        }

        public async UniTask<AddressableAsset<BreathingSettingsDb>> GetBreathingSettings() =>
             await ServiceLocator.Get<IAssetManager>().GetAssetAsync<BreathingSettingsDb>("LibraryBreathingExerciseDb");

        public async UniTask<AddressableAsset<BreathingSettingsDb>> GetDailyBreathingSettings() =>
            await ServiceLocator.Get<IAssetManager>().GetAssetAsync<BreathingSettingsDb>("DailyBreathingExerciseDb");
        
        public async UniTask<AddressableAsset<MoodDb>> GetMoodSettings() =>
            await ServiceLocator.Get<IAssetManager>().GetAssetAsync<MoodDb>("MoodDb");
        
        public async UniTask<int> Add<T>(T data) where T:IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                return await storage.Add<T>(data);
            }
            LogNotExistingStorage<T>();
            return -1;
        }
        
        public async UniTask<T> Get<T>(int id) where T:IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                return await storage.Get<T>(id);
            }

            LogNotExistingStorage<T>();
            return default;
        }

        public async UniTask<IEnumerable<T>> GetAll<T>() where T : IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                return await storage.GetAll<T>();
            }

            LogNotExistingStorage<T>();
            return Enumerable.Empty<T>();
        }
        
        public async UniTask Remove<T>(int id) where T:IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                await storage.Remove<T>(id);
                return;
            }

            LogNotExistingStorage<T>();
        }

        public async UniTask RemoveAll<T>()
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                await storage.RemoveAll<T>();
                return;
            }

            LogNotExistingStorage<T>();
        }

        private static void LogNotExistingStorage<T>() => 
            Log.LogError($"No storage for type {typeof(T)} exists", "DataManager");
    }

  
    public interface IStorage
    {
        UniTask<int> Add<T>(T data) where T : IDataObject;
        UniTask<T> Get<T>(int id) where T : IDataObject;
        UniTask<IEnumerable<T>> GetAll<T>() where T : IDataObject;
        UniTask Remove<T>(int id) where T : IDataObject;
        UniTask RemoveAll<T>();
    }

    public class LocalStorage : IStorage
    {
        public UniTask<int> Add<T>(T data) where T : IDataObject
        {
            var storageName = GetStorageNameForType<T>();
            var storageContentStr = PlayerPrefs.GetString(storageName, null);
            var storageContent = string.IsNullOrEmpty(storageContentStr) 
                ? new List<T>() 
                : JsonConvert.DeserializeObject<List<T>>(storageContentStr);

            int newId = storageContent.Count == 0 ? 0 : storageContent.Last().Id + 1;
            data.Id = newId;
            storageContent.Add(data);
            
            PlayerPrefs.SetString(storageName, JsonConvert.SerializeObject(storageContent));
            return new UniTask<int>(newId);
        }

        public UniTask<T> Get<T>(int id) where T : IDataObject
        {
            var storageContent = LoadStorage<T>();
            return new UniTask<T>(storageContent.FirstOrDefault(x => x.Id == id));
        }

        public UniTask<IEnumerable<T>> GetAll<T>() where T : IDataObject
        {
            var storageContent = LoadStorage<T>();
            return new UniTask<IEnumerable<T>>(storageContent);
        }

        public UniTask Remove<T>(int id) where T : IDataObject
        {
            var storageContent = LoadStorage<T>();
            int index = storageContent.FindIndex(x => x.Id == id);
            if (index == -1)
            {
                Log.LogError($"No such data object with id {id} exists", "DataManager");
                return new UniTask<T>(default);
            }
            
            storageContent.RemoveAt(index);
            SaveStorage<T>(storageContent);
            return UniTask.CompletedTask;
        }

        #if UNITY_EDITOR
        public static void RemoveAllEditor<T>()
        {
            PlayerPrefs.DeleteKey(GetStorageNameForType<T>());
        }

        public static void Dump<T>()
        {
            var content = PlayerPrefs.GetString(GetStorageNameForType<T>());
            Debug.Log($"=== Dump of {typeof(T)} ====");
            Debug.Log(content);
            Debug.Log($"=== End of Dump of {typeof(T)} ====");
        }
        
        #endif
        
        public UniTask RemoveAll<T>()
        {
            PlayerPrefs.DeleteKey(GetStorageNameForType<T>());
            return UniTask.CompletedTask;
        }

        private static string GetStorageNameForType<T>() => $"storage_{typeof(T)}";

        private static List<T> LoadStorage<T>()
        {
            var storageName = GetStorageNameForType<T>();
            var storageContentStr = PlayerPrefs.GetString(storageName, null);
            var storageContent = string.IsNullOrEmpty(storageContentStr) 
                ? new List<T>() 
                : JsonConvert.DeserializeObject<List<T>>(storageContentStr);
            return storageContent;
        }

        private static void SaveStorage<T>(List<T> storageContent) => 
            PlayerPrefs.SetString(GetStorageNameForType<T>(), JsonConvert.SerializeObject(storageContent));
    }
}