using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Meditation.Apis.Data;
using UnityEngine;

namespace Meditation.Apis
{
    public interface ISession
    {
        Action<int> BreathCountChanged { get; set; }

        UniTask Start(IBreathingSettings breathingSettings);
        UniTask Finish(float time);
        void IncreaseBreathingCountInSession();
    }
    
    public class BreathingSession : ISession
    {
        public Action<int> BreathCountChanged { get; set; }

        private IBreathingSettings actualBreathingSettings;
        private int breathsCountInSession;
        private State state = State.Prepared;
        private Calendar<FinishedBreathing> calendar;
        private enum State
        {
            Prepared,
            Running
        }

        public BreathingSession(Calendar<FinishedBreathing> calendar) => this.calendar = calendar;

        public UniTask Start(IBreathingSettings breathingSettings)
        {
            Debug.Assert(state == State.Prepared, $"Session must be in {state == State.Prepared} state but it is in {state}");
            state = State.Running;
            breathsCountInSession = 0;
            actualBreathingSettings = breathingSettings;
            return UniTask.CompletedTask;
        }

        public async UniTask Finish(float time)
        {
            if (actualBreathingSettings == null)
            {
                Debug.Log("[Breathing] actual session was closed");
                return;
            }
                
            Debug.Assert(state == State.Running,
                $"Session must be in {state == State.Running} state but it is in {state}");
            
          
            var finishedBreathing = new FinishedBreathing(
                actualBreathingSettings, 
                TimeSpan.FromSeconds(time), 
                breathsCountInSession);

            // save to data
            await ServiceLocator.Get<IDataManager>().Add(finishedBreathing);
            // update calendar
            calendar.AddEvent(finishedBreathing, finishedBreathing.DateTime);
            
            actualBreathingSettings = null;
            state = State.Prepared;
        }

        public void IncreaseBreathingCountInSession()
        {
            breathsCountInSession++;
            BreathCountChanged?.Invoke(breathsCountInSession);
        }
    }
}