using System.Collections.Generic;

namespace OneDay.Core.Sm
{
    public class StateData
    {
        private Dictionary<string, object> CustomData { get; set; }

        public static StateData Create(params (string key, object value)[] keysWithValues)
        {
            var stateData = new StateData
            {
                CustomData = new Dictionary<string, object>()
            };

            foreach (var keyValue in keysWithValues)
            {
                stateData.CustomData.Add(keyValue.key, keyValue.value);
            }

            return stateData;
        }
        
        public T GetValue<T>(string key)
        {
            if (CustomData == null)
                return default;

            if (CustomData.TryGetValue(key, out var data))
            {
                return (T)data;
            }

            return default;
        }
    }
}