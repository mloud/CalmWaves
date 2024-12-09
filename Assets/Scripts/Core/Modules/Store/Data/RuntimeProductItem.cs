using UnityEngine.Purchasing;

namespace OneDay.Core.Modules.Store.Data
{
    public class RuntimeProductItem
    {
        public Product Product { get; }
        public ProductSettings Setting { get; }
        
        public RuntimeProductItem(Product product, ProductSettings setting)
        {
            Product = product;
            Setting = setting;
        }
    }
}