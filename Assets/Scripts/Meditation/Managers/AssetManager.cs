using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Meditation
{
    public interface IAssetManager
    {
        UniTask<AddressableAsset<T>> GetAssetAsync<T>(string key);
    }
    public class AssetManager : MonoBehaviour, IAssetManager, IService
    {
        public UniTask Initialize() => UniTask.CompletedTask;
        /// <summary>
        /// Load an asset of type T by its address key.
        /// </summary>
        public async UniTask<AddressableAsset<T>> GetAssetAsync<T>(string key)
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return new AddressableAsset<T>(handle);
            }

            Debug.LogError($"No such addressable with key {key} exists");
            return null;
        }
    }
    
    public interface IReleasable
    {
        void Release();
    }
    
    public interface IAsset<out T>
    {
        T GetReference();
    }
    
    public class AddressableAsset<T> : IAsset<T>, IReleasable
    {
        private AsyncOperationHandle Handle { get; }
        public AddressableAsset(AsyncOperationHandle handle) => Handle = handle;
        public void Release()
        {
            if (Handle.IsValid())
            {
                Addressables.Release(Handle);       
            }
        }
        public T GetReference() => (T)Handle.Result;
    }
}