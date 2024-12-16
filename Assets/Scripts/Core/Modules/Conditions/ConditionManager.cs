using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core.Debugging;
using UnityEngine;

namespace OneDay.Core.Modules.Conditions
{
    public interface ICondition
    {
        UniTask<bool> Evaluate();
    }
 
    public interface IConditionManager
    {
        UniTask<bool> Evaluate(string conditionName);
        void RegisterCondition(string name, Func<UniTask<bool>> method);
        void RegisterCondition(string name, Func<bool> method);
        void RegisterCondition(string name, ICondition method);
    }
    
    [LogSection("Condition")]
    public class ConditionManager : MonoBehaviour, IService, IConditionManager
    {
        public UniTask Initialize() => UniTask.CompletedTask;
        public UniTask PostInitialize() => UniTask.CompletedTask;
     
        private Dictionary<string, Func<UniTask<bool>>> conditions = new();

        public async UniTask<bool> Evaluate(string conditionName)
        {
            if (conditions.TryGetValue(conditionName, out var condition))
            {
                var result = await condition.Invoke();
                D.LogInfo($"Condition {conditionName} evaluated: {result}", this);
                return result;
            }

            D.LogError($"Condition {conditionName} is not registered", this);
            return false;
        }

        public void RegisterCondition(string conditionName, Func<UniTask<bool>> method)
        {
            if (conditions.TryAdd(conditionName, method)) 
                return;
            Debug.LogError($"Condition {conditionName} is already registered");
        }

        public void RegisterCondition(string conditionName, Func<bool> method)
        {
            if (conditions.TryAdd(conditionName, () => UniTask.FromResult(method())))
                return;
            Debug.LogError($"Condition {conditionName} is already registered");
        }

        public void RegisterCondition(string conditionName, ICondition method)
        {
            if (conditions.TryAdd(conditionName, method.Evaluate)) 
                return;
            Debug.LogError($"Condition {conditionName} is already registered");
        }
    }
}