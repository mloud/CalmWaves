using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OneDay.Core.Modules.Update
{
    public interface IUpdateManager
    {
        void RegisterUpdate(Action<float> updateMethod);
        void RegisterLateUpdate(Action<float> updateMethod);
        void RegisterFixedUpdate(Action<float> updateMethod);
        bool UnregisterUpdate(Action<float> updateMethod);
        bool UnregisterLateUpdate(Action<float> updateMethod);
        bool UnregisterFixedUpdate(Action<float> updateMethod);
    }

    public class UpdateManager : MonoBehaviour, IUpdateManager, IService
    {
        private readonly List<Action<float>> updateMethods = new();
        private readonly List<Action<float>> lateUpdateMethods = new();
        private readonly List<Action<float>> fixedUpdateMethods = new();

        public async UniTask Initialize()
        {
            await UniTask.CompletedTask; // Simulate asynchronous initialization
        }

        public void RegisterUpdate(Action<float> updateMethod)
        {
            if (!updateMethods.Contains(updateMethod))
            {
                updateMethods.Add(updateMethod);
            }
        }

        public void RegisterLateUpdate(Action<float> updateMethod)
        {
            if (!lateUpdateMethods.Contains(updateMethod))
            {
                lateUpdateMethods.Add(updateMethod);
            }
        }

        public void RegisterFixedUpdate(Action<float> updateMethod)
        {
            if (!fixedUpdateMethods.Contains(updateMethod))
            {
                fixedUpdateMethods.Add(updateMethod);
            }
        }
        
        public bool UnregisterUpdate(Action<float> updateMethod) => updateMethods.Remove(updateMethod);

        public bool UnregisterLateUpdate(Action<float> updateMethod) => lateUpdateMethods.Remove(updateMethod);

        public bool UnregisterFixedUpdate(Action<float> updateMethod) => fixedUpdateMethods.Remove(updateMethod);

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            foreach (var method in updateMethods)
            {
                method.Invoke(deltaTime);
            }
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;

            foreach (var method in lateUpdateMethods)
            {
                method.Invoke(deltaTime);
            }
        }

        private void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;

            foreach (var method in fixedUpdateMethods)
            {
                method.Invoke(fixedDeltaTime);
            }
        }
    }
}