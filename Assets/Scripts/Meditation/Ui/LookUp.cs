using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meditation.Ui
{
   public static class LookUp
    {
        private static Dictionary<Type, ILookUp> LookUps { get; } = new();

        public static LookUp<T> Get<T>() where T: Component
        {
            if (LookUps.TryGetValue(typeof(T), out var lookup))
            {
                return (LookUp<T>)lookup;
            }

            var lookUp = new LookUp<T>();
            LookUps.Add(typeof(T), lookUp);
            return lookUp;
        }
    }

    public interface ILookUp
    {
    }

    public class LookUp<T>: ILookUp where T: Component
    {
        private List<T> Cache { get; set; } = new();
        
        public void Register(T component)
        {
            Cache.Add(component);    
        }

        public void Unregister(T component)
        {
            Cache.Remove(component);
        }

        public T GetFirst() => Cache.FirstOrDefault();
        public T GetFirstWihName(string name) => Cache.Find(x => x.name == name);
        public List<T> GetAll(string name = null) => Cache.FindAll(x => name == null || x.name == name);
    }
}