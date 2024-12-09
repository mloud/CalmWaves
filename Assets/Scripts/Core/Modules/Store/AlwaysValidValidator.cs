namespace OneDay.Core.Modules.Store
{
    public class AlwaysValidValidator : IProductPurchaseValidator
    {
        public bool Validate(string _) => true;
    }
}