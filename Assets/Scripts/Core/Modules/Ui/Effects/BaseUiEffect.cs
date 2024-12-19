using System;
using OneDay.Core;
using UnityEngine;

namespace Core.Modules.Ui.Effects
{
    public abstract class BaseUiEffect : MonoBehaviour, IEffect
    {
        public string Id => id;
        
        [SerializeField] private string id;

        private void Start()
        {
            ServiceLocator.Get<IEffectManager>().RegisterEffect(this);
        }

        public abstract void Run();
        public abstract void Stop();
        public abstract bool IsPlaying();
    }
}