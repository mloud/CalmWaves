using System;
using OneDay.Core.Modules.Data;
using UnityEngine;
using UnityEngine.Purchasing;

namespace OneDay.Core.Modules.Store.Data
{
    [Serializable]
    public class ProductSettings : BaseDataObject
    {
        public string ProductId;
        public string Name;
        public string Description;
        public string Label;
        public ProductType ItemType;
    }
    
    [CreateAssetMenu(fileName = "StoreItemSettings", menuName = "ScriptableObjects/StoreItemSettings", order = 1)]

    public class ProductSettingsTable : ScriptableObjectTable<ProductSettings>
    { }
}