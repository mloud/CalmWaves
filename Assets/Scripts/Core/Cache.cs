using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core
{
    public abstract class Cache
    {
        private TimeSpan CacheDuration { get; set; }
        private DateTime CacheLastUpdate { get; set; }
        private bool CanExpire { get; set; }

        protected Cache() => CanExpire = false;

        protected Cache(TimeSpan cacheDuration)
        {
            CacheDuration = cacheDuration;
            CanExpire = true;
        }
        protected bool IsExpired() => 
            CanExpire && DateTime.Now > CacheLastUpdate + CacheDuration;
 
        protected void MarkCacheLastUpdate() => CacheLastUpdate = DateTime.Now;

        public abstract void Clear();
    }
    

    public class SyncCache<T> : Cache
    {
        private T CachedValue { get; set; }
        private Func<T> ValueGetter { get; set; }
        
        public SyncCache(Func<T> valueGetter) => 
            ValueGetter = valueGetter;

        public SyncCache(Func<T> valueGetter, TimeSpan cacheDuration) 
            : base(cacheDuration) => 
            ValueGetter = valueGetter;
        
        public T Get()
        {
            if (CachedValue != null && !IsExpired())
            {
                return CachedValue;
            }

            CachedValue = ValueGetter();
            MarkCacheLastUpdate();
            return CachedValue;
        }

        public override void Clear()
        {
            CachedValue = default;
        }

        public void Preload() => Get();
    }
    
    
    public class AsyncCache<T> : Cache
    {
        private T CachedValue { get; set; }
        private Func<UniTask<T>> ValueGetter { get; set; }
        
        public AsyncCache(Func<UniTask<T>> valueGetter) => ValueGetter = valueGetter;

        public AsyncCache(Func<UniTask<T>> valueGetter, TimeSpan cacheDuration) 
            : base(cacheDuration) => 
            ValueGetter = valueGetter;

        public T GetSync()
        {
            if (CachedValue == null || IsExpired())
                throw new InvalidOperationException(
                    "Value is not cached or it is expired, cannot use sync method to retrieve the value");
            return CachedValue;
        }
        
        public async UniTask<T> Get()
        {
            if (CachedValue != null && !IsExpired())
            {
                return CachedValue;
            }

            try
            {
                CachedValue = await ValueGetter();
                MarkCacheLastUpdate();
            }
            catch
            {
                Debug.LogError("Error while get value from origin");
                throw;
            }

            return CachedValue;
        }

        public override void Clear() => CachedValue = default;

        public async UniTask Preload() => await Get();
    }
}