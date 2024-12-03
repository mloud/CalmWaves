using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Assets;

namespace OneDay.Core.Modules.Data
{
    public class AddressableScriptableObjectStorage : IStorage
    {
        private Dictionary<Type, string> typeToKeyBindings = new();

        public void RegisterTypeToKeyBinding<T>(string key)
        {
            typeToKeyBindings.Add(typeof(T), key);
        }

        public UniTask<int> Add<T>(T data) where T : IDataObject =>
            throw new NotSupportedException("Saving to scriptable object storage is not allowed");

        public UniTask<bool> Actualize<T>(T data) where T : IDataObject =>
            throw new NotSupportedException("Actualize to scriptable object storage is not allowed");

        public async UniTask<T> Get<T>(int id) where T : IDataObject
        {
            var storageContent = await LoadStorage<T>();
            return storageContent.Data.Find(x => x.Id == id);
        }

        public async UniTask<IEnumerable<T>> GetAll<T>() where T : IDataObject
        {
            var storageContent = await LoadStorage<T>();
            return storageContent.Data;
        }

        public UniTask Remove<T>(int id) where T : IDataObject =>
            throw new NotSupportedException("Remove from scriptable object storage is not allowed");

        public UniTask RemoveAll<T>() =>
            throw new NotSupportedException("Remove all from scriptable object storage is not allowed");


        private string GetStorageNameForType<T>() => typeToKeyBindings[typeof(T)];

        private async UniTask<ScriptableObjectTable<T>> LoadStorage<T>() where T : IDataObject
        {
            var storageName = GetStorageNameForType<T>();
            var addressableAsset = await ServiceLocator.Get<IAssetManager>()
                .GetAssetAsync<ScriptableObjectTable<T>>(storageName);

            return addressableAsset.GetReference();
        }
    }
}