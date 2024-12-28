using System.Collections.Generic;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace Meditation.Data
{
    [System.Serializable]
    public class BreathingPhaseDescription : BaseDataObject
    {
        public string GetBreathingPhase() => breathingPhase;

        [SerializeField] private string breathingPhase;
        [SerializeField] private List<string> descriptions;
        [SerializeField] private List<float> durations;

        public string GetDescription(float duration)
        {
            for (int i = 0; i < durations.Count; i++)
            {
                if (duration <= durations[i])
                    return descriptions[i];
            }
            return descriptions[durations.Count - 1];
        }
    }
    
    [CreateAssetMenu(fileName = "BreathingPhaseDescriptionTable", menuName = "ScriptableObjects/BreathingPhaseDescriptions", order = 1)]

    public class BreathingPhaseDescriptionTable : ScriptableObjectTable<BreathingPhaseDescription>
    { }
}