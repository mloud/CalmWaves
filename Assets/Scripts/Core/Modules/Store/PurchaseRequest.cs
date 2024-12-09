using UnityEngine.Purchasing;

namespace OneDay.Core.Modules.Store
{
    public class PurchaseRequest
    {
        public enum State { InProgress, Ok, Failed }
        public Product Product { get; }
        public State PurchaseState { get; private set; }
        public string Error { get; private set; }
            
        public PurchaseRequest(Product product)
        {
            Product = product;
            PurchaseState = State.InProgress;
        }

        public void MarkedAsPurchased() => PurchaseState = State.Ok;

        public void MarkAsFailed(string error)
        {
            Error = error;
            PurchaseState = State.Failed;
        }
    }
}