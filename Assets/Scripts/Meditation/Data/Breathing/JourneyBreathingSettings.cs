using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meditation.Data.Breathing
{
    [Serializable]
    public class JourneyBreathingSettings : IBreathingSettings
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string intervals;
        [SerializeField] private int rounds;

        private List<float> internalIntervals;

        private string customName;
        public string GetName() => !string.IsNullOrEmpty(customName) ? customName : name;
        public string GetIcon() => null;
        public int Rounds
        {
            get => rounds;
            set => rounds = value;
        }

        public void SetCustomName(string customName) => this.customName = customName;
        public string GetDescription() => description;
        public string GetLabel() => "Journey";
        public float GetInhaleDuration() => GetInterval(0);
        public float GetAfterInhaleDuration() =>  GetInterval(1);
        public float GetExhaleDuration() =>  GetInterval(2);
        public float GetAfterExhaleDuration() =>  GetInterval(3);
        public float GetTotalTime() => Rounds * GetOneBreatheTime();
      
        public float GetOneBreatheTime() => 
            GetInhaleDuration() + 
            GetAfterInhaleDuration() + 
            GetExhaleDuration() +
            GetAfterExhaleDuration();

        private float GetInterval(int index)
        {
            Debug.Assert(!string.IsNullOrEmpty(intervals), $"Cannot parse intervals in {GetName()}");
            if (internalIntervals == null || internalIntervals.Count == 0)
            {
                var split = intervals.Split(',');
                internalIntervals = split.Select(x => float.Parse(x)).ToList();
            }
        

            Debug.Assert(internalIntervals.Count == 4, $"Cannot parse journey index {index} in mission {GetName()}");
            return internalIntervals[index];
        }
    }
}