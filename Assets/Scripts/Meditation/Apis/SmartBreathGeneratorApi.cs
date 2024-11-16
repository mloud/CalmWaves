using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Meditation.Apis
{
    public interface IBreathGeneratorApi
    {
        UniTask<IBreathingSettings> Generate(IEnumerable<string> moods);
    }
    
    public class SmartBreathGeneratorApi: MonoBehaviour, IBreathGeneratorApi, IService
    {
        [SerializeField] private string url;
        [TextArea(1, 50)]
        [SerializeField] private string prompt;
        private IPromptService promptService;
 
        public UniTask Initialize()
        {
            promptService = new PromptService(url);
            return UniTask.CompletedTask;
        }

        public async UniTask<IBreathingSettings> Generate(IEnumerable<string> moods)
        {
            var smartBreathResult = new SmartBreathResult();

            var filledPrompt = prompt
                .Replace("{mood}", string.Join(',', moods))
                .Replace("{result}", smartBreathResult.ToString());
            
            var response = await promptService.SendRequest(filledPrompt);
            
            if (response != null)
            {
                response = response
                    .Replace("```json", "")
                    .Replace("```", "");
                try
                {
                    smartBreathResult = JsonConvert.DeserializeObject<SmartBreathResult>(response);
                }
                catch (JsonReaderException ex)
                {
                    Debug.LogError(ex.ToString());
                    return null;
                }

                return new SmartBreathingSettings(smartBreathResult);
            }
            return null;
        }

        private class SmartBreathingSettings : IBreathingSettings
        {
            private string name;
            private string description;
            private string music;
            private BreathingTiming breathingTiming;
            private BreathingTargetTime breathingTargetTime;
            private int rounds;
            private string label;

            public SmartBreathingSettings(SmartBreathResult result)
            {
                name = result.name;
                description = result.description;
                breathingTiming = new BreathingTiming
                {
                    InhaleDuration = result.inhaleDuration,
                    AfterInhaleDuration = result.afterInhaleHoldDuration,
                    ExhaleDuration = result.exhaleDuration,
                    AfterExhaleDuration = result.afterExhaleHoldDuration
                };
                rounds = result.rounds;
                label = "ai";
            }
            public string GetName() => name;
            public string GetDescription() => description;
            public string GetMusic() => music;
            public string GetLabel() => string.IsNullOrEmpty(label) ? null : label;
            public float GetInhaleDuration() => breathingTiming.InhaleDuration;
            public float GetAfterInhaleDuration() => breathingTiming.AfterInhaleDuration;
            public float GetExhaleDuration() => breathingTiming.ExhaleDuration;
            public float GetAfterExhaleDuration() => breathingTiming.AfterExhaleDuration;
            public BreathingTiming GetBreathingTiming()  => breathingTiming;
            public BreathingTargetTime GetBreathingTargetTime() => breathingTargetTime;
            public float GetTotalTime() =>
                (breathingTiming.InhaleDuration + 
                 breathingTiming.AfterInhaleDuration + 
                 breathingTiming.ExhaleDuration + 
                 breathingTiming.AfterExhaleDuration) * rounds;
            public int Rounds() => rounds;
        }
        
        private class SmartBreathResult
        {
            public string name;
            public string description;
            public float inhaleDuration;
            public float afterInhaleHoldDuration;
            public float exhaleDuration;
            public float afterExhaleHoldDuration;
            public int rounds;

            // Method to convert the object to a string representation
            public override string ToString()
            {
                return $"{{ " +
                       $"\"name\": \"{name}\", " +
                       $"\"description\": \"{description}\", " +
                       $"\"inhaleDuration\": {inhaleDuration}, " +
                       $"\"afterInhaleHoldDuration\": {afterInhaleHoldDuration}, " +
                       $"\"exhaleDuration\": {exhaleDuration}, " +
                       $"\"afterExhaleHoldDuration\": {afterExhaleHoldDuration}, " +
                  //     $"\"rounds\": {rounds} " +
                       $"}}";
            }
        }
    }
}