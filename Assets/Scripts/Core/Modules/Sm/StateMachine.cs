using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Sm
{
    public class StateMachine
    {
        private IState currentState;

        private Dictionary<Type, IState> states = new();

        /// <summary>
        /// Set a new state and handle the transition asynchronously.
        /// </summary>
        public async UniTask SetStateAsync<T>(StateData stateData = null, bool waitForCurrentStateExit = true) where T: IState
        {
            if (!states.TryGetValue(typeof(T), out var newState))
            {
                Debug.LogError($"No such state {typeof(T)} exists");
                return;
            }
            
            if (currentState != null)
            {
                if (waitForCurrentStateExit)
                {
                    await currentState.ExitAsync();
                }
                else
                {
                    currentState.ExitAsync().Forget();
                }
            }

            currentState = newState;

            if (currentState != null)
            {
                await currentState.EnterAsync(stateData);
                await currentState.ExecuteAsync();
            }
        }

        public async UniTask RegisterState<T> () where T: IState, new()
        {
            var state = new T
            {
                StateMachine = this
            };
            states.Add(typeof(T), state);
            await state.Initialize();
        }
    }
}