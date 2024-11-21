using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meditation.Ui
{
    public interface IUiParameter
    {
        IUiParameter Add(string key, object data);
        IUiParameter Remove(string key);
        T Get<T>(string key);
        T GetFirst<T>();

    }
    
    public class UiParameter : IUiParameter
    {
        private Dictionary<string, object> keyData = new();

        public static IUiParameter Create(string key, object value)
        {
            var parameter = new UiParameter();
            return parameter.Add(key, value);
        }
        
        public static IUiParameter Create<T>(T value)
        {
            var parameter = new UiParameter();
            return parameter.Add(typeof(T).ToString(), value);
        }
        
        public IUiParameter Add(string key, object data)
        {
            Debug.Assert(!keyData.ContainsKey(key));
            keyData.Add(key, data);
            return this;
        }

        public IUiParameter Remove(string key)
        {
            Debug.Assert(keyData.ContainsKey(key));
            keyData.Remove(key);
            return this;
        }

        public T Get<T>(string key)
        {
            if (keyData.TryGetValue(key, out var data))
            {
                return (T)data;
            }
            return default;
        }

        public T GetFirst<T>() => (T)keyData.Values.FirstOrDefault(x => x.GetType() == typeof(T));
    }
}