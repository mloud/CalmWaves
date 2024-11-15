using System.Collections.Generic;
using UnityEngine;

namespace Meditation
{
    [CreateAssetMenu(fileName = "BreathingExerciseDb", menuName = "Breathing/BreathingExerciseDb")]
    public class BreathingSettingsDb : ScriptableObject
    {
        [SerializeField] private List<BreathingSettings> builtInSettings;
        
        public IBreathingSettings Get(string settingsName)
        {
            var settingsList = builtInSettings.FindAll(x => x.GetName() == settingsName);
            switch (settingsList.Count)
            {
                case 0:
                    Debug.Log($"No such breathing settings {settingsName} found");
                    return null;
                case > 1:
                    Debug.Log($"More than one breathing settings {settingsName} found, returning the first one");
                    return settingsList[0];
                default:
                    return settingsList[0];
            }
        }

        public IEnumerable<IBreathingSettings> GetAll() => builtInSettings;
    }
}