using Meditation.Apis.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meditation
{
    [CreateAssetMenu(fileName = "NewBreathingExercise", menuName = "Breathing/BreathingExercise")]
    public class BreathingSettings : ScriptableObject, IBreathingSettings
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string music;
        [SerializeField] private BreathingTiming breathingTiming;
        [SerializeField] private BreathingTargetTime breathingTargetTime;
        [SerializeField] private int rounds;
        [SerializeField] private string label;
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
}