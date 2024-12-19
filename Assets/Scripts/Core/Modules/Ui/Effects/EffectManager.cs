using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using UnityEngine;

namespace Core.Modules.Ui.Effects
{
    public interface IEffectManager
    {
        IEnumerable<IEffect> GetEffects(string id);
        IEffect GetFirstEffect(string id);
        
        void RegisterEffect(IEffect effect);
        void UnregisterEffect(IEffect effect);
    }
    
    public class EffectManager : MonoBehaviour, IService, IEffectManager
    {
        private List<IEffect> registeredEffects = new();

        public UniTask Initialize() => UniTask.CompletedTask;

        public UniTask PostInitialize() => UniTask.CompletedTask;

        public IEnumerable<IEffect> GetEffects(string id) => 
            registeredEffects.Where(x => x.Id == id);

        public IEffect GetFirstEffect(string id) => 
            registeredEffects.FirstOrDefault(x => x.Id == id);
       
        public void RegisterEffect(IEffect effect)
        {
            Debug.Assert(!registeredEffects.Contains(effect));
            registeredEffects.Add(effect);
        }

        public void UnregisterEffect(IEffect effect)
        {
            Debug.Assert(registeredEffects.Contains(effect));
            registeredEffects.Remove(effect);
        }
    }
}