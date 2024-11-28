using System;
using Meditation.Apis.Data;
using OneDay.Core.Modules.Data;

namespace Meditation.Data
{
   public class CustomBreathingSettings : BaseDataObject, IBreathingSettings
   {
      public static CustomBreathingSettings Default() => new CustomBreathingSettings
      {
         Name = "",
         BreathingTiming = new BreathingTiming
         {
            AfterInhaleDuration = 4,
            AfterExhaleDuration = 4,
            ExhaleDuration = 4,
            InhaleDuration = 4,
         },
         Rounds = 12,
         CreateTime = DateTime.Now
      };
      public string Name { get; set; }
      public DateTime CreateTime { get; set; }
      public BreathingTiming BreathingTiming { get; set; }
      public BreathingTargetTime BreathingTargetTime { get; set; }

      public string GetName() => Name;
      public string GetIcon() => null;
      public string GetDescription() => null;
      public string GetMusic() => "";
      public string GetLabel() => "Custom";
      public float GetInhaleDuration() => BreathingTiming.InhaleDuration;
      public float GetAfterInhaleDuration() => BreathingTiming.AfterInhaleDuration;
      public float GetExhaleDuration() => BreathingTiming.ExhaleDuration;
      public float GetAfterExhaleDuration() => BreathingTiming.AfterExhaleDuration;
      public float GetTotalTime() => Rounds * GetOneBreatheTime();
      
      public float GetOneBreatheTime() => 
         GetInhaleDuration() + 
         GetAfterInhaleDuration() + 
         GetExhaleDuration() +
         GetAfterExhaleDuration();

      public BreathingTiming GetBreathingTiming() => BreathingTiming;

      public BreathingTargetTime GetBreathingTargetTime() => BreathingTargetTime;
      
      public int Rounds { get; set; }
   }
}