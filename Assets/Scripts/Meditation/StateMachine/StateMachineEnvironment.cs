using System.Collections.Generic;

namespace Meditation.States
{
    public static class StateMachineEnvironment
    {
        public static StateMachine Default { get; private set; }

        private static Dictionary<string, StateMachine> StateMachines { get; set; }

        public static void RegisterStateMachine(string name, StateMachine stateMachine, bool isDefault)
        {
            StateMachines ??= new Dictionary<string, StateMachine>();
            StateMachines.Add(name, stateMachine);
            if (isDefault)
            {
                Default = stateMachine;
            }
        }

        public static StateMachine Get(string name)
        {
            StateMachines.TryGetValue(name, out var stateMachine);
            return stateMachine;
        }

        public static void UnregisterStateMachine(string name)
        {
            if (StateMachines.TryGetValue(name, out var stateMachine))
            {
                if (Default == stateMachine)
                    Default = null;
                StateMachines.Remove(name);
            }
        }

        public static void UnregisterAll()
        {
            Default = null;
            StateMachines?.Clear();
        }
    }
}