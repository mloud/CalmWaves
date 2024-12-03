using System.Collections.Generic;
using UnityEngine;

namespace OneDay.Core.Modules.Data
{
    public abstract class ScriptableObjectTable<T> : ScriptableObject, ITable<T> where T: IDataObject
    {
        [SerializeField] private List<T> data;
        public List<T> Data => data;
    }
}