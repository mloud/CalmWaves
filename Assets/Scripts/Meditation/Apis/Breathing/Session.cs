using System;
using Meditation.Apis.Data;
using Meditation.Data;
using UnityEngine;

namespace Meditation.Apis
{
    public interface ISession
    {
        Action<int> BreathCountChanged { get; set; }

        void Start(IBreathingSettings breathingSettings);
        FinishedBreathing Finish(float time);
        void IncreaseBreathingCountInSession();
    }
    
    public class BreathingSession : ISession
    {
        public Action<int> BreathCountChanged { get; set; }

        private IBreathingSettings actualBreathingSettings;
        private IBreathingApi breathingApi;
        
        private int breathsCountInSession;
        private State state = State.Prepared;
     
        private enum State
        {
            Prepared,
            Running
        }

        public BreathingSession(IBreathingApi breathingApi) => this.breathingApi = breathingApi;

        public void Start(IBreathingSettings breathingSettings)
        {
            Debug.Assert(state == State.Prepared, $"Session must be in {State.Prepared} state but it is in {state}");
            state = State.Running;
            breathsCountInSession = 0;
            actualBreathingSettings = breathingSettings;
        }

        public FinishedBreathing Finish(float time)
        {
            if (actualBreathingSettings == null)
            {
                Debug.Log("[Breathing] actual session was closed");
                state = State.Prepared;
                return null;
            }
                
            Debug.Assert(state == State.Running,
                $"Session must be in {state == State.Running} state but it is in {state}");
            
            var finishedBreathing = new FinishedBreathing(
                actualBreathingSettings, 
                TimeSpan.FromSeconds(time), 
                breathsCountInSession);

            actualBreathingSettings = null;
            state = State.Prepared;
            return finishedBreathing;
        }

        public void IncreaseBreathingCountInSession()
        {
            breathsCountInSession++;
            BreathCountChanged?.Invoke(breathsCountInSession);
        }
    }
}