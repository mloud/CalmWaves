using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace OneDay.Core
{
    public static class UnityGS
    {
        public static async UniTask Initialize(string environment)
        {
            try
            {
                var options = new InitializationOptions()
                    .SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                throw;
            }
        }
    }
}