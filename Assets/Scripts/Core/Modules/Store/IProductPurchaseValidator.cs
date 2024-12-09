namespace OneDay.Core.Modules.Store
{
    public interface IProductPurchaseValidator
    {
        bool Validate(string receipt);
    }
}