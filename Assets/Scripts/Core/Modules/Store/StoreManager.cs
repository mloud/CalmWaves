using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Store.Data;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace OneDay.Core.Modules.Store
{
    public interface IStoreManager
    {
        UniTask<IResult<RuntimeProductItem>> BuyProduct(string productId);

        IEnumerable<RuntimeProductItem> GetProducts(ProductType? productType);
    }

    public class StoreManager : MonoBehaviour, IService, IStoreManager, IDetailedStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider storeExtensionProvider;
        private IDataManager dataManager;
        private PurchaseRequest PurchaseRequest { get; set; }

        private List<RuntimeProductItem> registeredProducts;
        private List<ProductSettings> productSettings;
        
        public async UniTask Initialize()
        {
            if (IsInitialized())
                return;

            dataManager = ServiceLocator.Get<IDataManager>();
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
   
            productSettings = (await dataManager.GetAll<ProductSettings>()).ToList();
            productSettings.ForEach(x=>
                builder.AddProduct(x.ProductId, x.ItemType , new IDs {
                { x.ProductId, GooglePlay.Name },
                /*{ "com.yourapp.weekly", AppleAppStore.Name }*/ 
                }));
        
            UnityPurchasing.Initialize(this, builder);
        }
        
        public UniTask PostInitialize() => UniTask.CompletedTask;

 
        public async UniTask<IResult<RuntimeProductItem>> BuyProduct(string productId)
        {
            if (!IsInitialized())
            {
                Debug.LogError("Unity IAP not initialized");
                return Result<RuntimeProductItem>.CreateError("StoreManager is not initialized");
            }

            if (PurchaseRequest != null)
            {
                return Result<RuntimeProductItem>.CreateError(
                    $"Purchase already in progress {PurchaseRequest.Product.definition.id}");
            }

            var setting = productSettings.Find(x => x.ProductId == productId);
            
            if (setting == null)
            {
                return Result<RuntimeProductItem>.CreateError(
                    $"No product settings found for product {productId}");
            }
            
            var product = storeController.products.WithID(productId);

            if (product == null || !product.availableToPurchase)
            {
                Debug.LogError("Product not available for purchase: " + productId);
                return Result<RuntimeProductItem>.CreateError("Product not available for purchase: " + productId);
            }

            PurchaseRequest = new PurchaseRequest(product);
            storeController.InitiatePurchase(product);

            await UniTask.WaitUntil(() => PurchaseRequest.PurchaseState != PurchaseRequest.State.InProgress);

            return PurchaseRequest.PurchaseState == PurchaseRequest.State.Ok
                ? Result<RuntimeProductItem>.CreateSuccessful(
                    new RuntimeProductItem(PurchaseRequest.Product, setting))
                : Result<RuntimeProductItem>.CreateError($"Purchase failed with error: {PurchaseRequest.Error}");
        }

        public IEnumerable<RuntimeProductItem> GetProducts(ProductType? productType) =>
            productType == null 
                ? registeredProducts 
                : registeredProducts
                    .Where(x => x.Setting.ItemType == productType);
        
        public void OnInitializeFailed(InitializationFailureReason error) => 
            Debug.LogError($"Initialization failed error: {error}");

        public void OnInitializeFailed(InitializationFailureReason error, string message) => 
            Debug.LogError($"Initialization failed error: {error} message: {message}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log($"ProcessPurchase {purchaseEvent.purchasedProduct.definition.id}");
            PurchaseRequest?.MarkedAsPurchased();
            PurchaseRequest = null;
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError($"PurchaseFailed " +
                           $"product: {product}" +
                           $"reason: {failureReason}");
            
            PurchaseRequest?.MarkAsFailed(failureReason.ToString());
            PurchaseRequest = null;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log($"Initialized");
            Debug.Assert(productSettings != null);
            storeController = controller;
            storeExtensionProvider = extensions;
            registeredProducts = productSettings.Select(x =>
                new RuntimeProductItem(storeController.products.WithID(x.ProductId), x))
                .ToList();
            
            Debug.Log(JsonConvert.SerializeObject(registeredProducts));
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"PurchaseFailed " +
                           $"product: {product}" +
                           $"reason: {failureDescription.reason} " +
                           $"description: {failureDescription.message}");
            
            PurchaseRequest?.MarkAsFailed(failureDescription.message);
        }

        private bool IsInitialized() => 
            storeController != null && storeExtensionProvider != null;
    }
}