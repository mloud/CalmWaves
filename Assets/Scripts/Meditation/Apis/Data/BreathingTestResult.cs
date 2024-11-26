using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OneDay.Core.Modules.Data;

namespace Meditation.Apis.Data
{
    public class BreathingTestResult : BaseDataObject
    {
        public Dictionary<string, TimeSpan> Tests { get; set; }
        public DateTime Date { get; set; }
        [JsonConstructor]
        
        public BreathingTestResult() { }
       
        public BreathingTestResult(DateTime date,params (string key, TimeSpan value)[] results)
        {
            Date = date;
            Tests = results.ToDictionary(item => item.key, item => item.value);
        }
    }
}