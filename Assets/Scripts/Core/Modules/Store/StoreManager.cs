using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core.Debugging;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Store.Data;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace OneDay.Core.Modules.Store
{
    public interface IStoreManager
    {
        Action<string> ProductPurchased { get; set; }
        UniTask<IResult<RuntimeProductItem>> BuyProduct(string productId);

        IEnumerable<RuntimeProductItem> GetProducts(ProductType? productType);

        bool IsSubscriptionActive(string subscriptionId);
    }
    
    [LogSection("Store")]
    public class StoreManager : MonoBehaviour, IService, IStoreManager, IDetailedStoreListener
    {
        public Action<string> ProductPurchased { get; set; }
        public IProductPurchaseValidator Validator { get; set; }
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
            productSettings.ForEach(x =>
                {
                    builder.AddProduct(x.ProductId, x.ItemType, new IDs
                    {
                        { x.ProductId, GooglePlay.Name },
                        /*{ "com.yourapp.weekly", AppleAppStore.Name }*/
                    });
                    D.LogInfo($"Adding product: {x.ProductId} od type: {x.ItemType} to the builder");
                }
            );
        
            UnityPurchasing.Initialize(this, builder);
        }
        
        public UniTask PostInitialize() => UniTask.CompletedTask;

        public async UniTask<IResult<RuntimeProductItem>> BuyProduct(string productId)
        {
            if (!IsInitialized())
            {
                D.LogError("Unity IAP not initialized", this);
                return Result<RuntimeProductItem>.CreateError("StoreManager is not initialized");
            }

            if (PurchaseRequest != null && PurchaseRequest.PurchaseState == PurchaseRequest.State.InProgress)
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
                D.LogError("Product not available for purchase: " + productId, this);
                return Result<RuntimeProductItem>.CreateError("Product not available for purchase: " + productId);
            }

            PurchaseRequest = new PurchaseRequest(product);
            storeController.InitiatePurchase(product);

            await UniTask.WaitUntil(() => PurchaseRequest.PurchaseState != PurchaseRequest.State.InProgress);

            bool wasSuccessful = PurchaseRequest.PurchaseState == PurchaseRequest.State.Ok;
            
            if (wasSuccessful)
            {
                ProductPurchased?.Invoke(productId);
            }
            
            return wasSuccessful
                ? Result<RuntimeProductItem>.CreateSuccessful(
                    new RuntimeProductItem(PurchaseRequest.Product, setting))
                : Result<RuntimeProductItem>.CreateError($"Purchase failed with error: {PurchaseRequest.Error}");
        }

        public IEnumerable<RuntimeProductItem> GetProducts(ProductType? productType) =>
            productType == null 
                ? registeredProducts 
                : registeredProducts
                    .Where(x => x.Setting.ItemType == productType);

        public bool IsSubscriptionActive(string subscriptionId)
        {
            if (storeController == null) return false;
            var product = storeController.products.WithID(subscriptionId);

            if (product != null && product.hasReceipt)
            {
                if (Validator != null) 
                    return Validator.Validate(product.receipt);
                Debug.LogWarning("Validation skipped - no validator set");
                return true;
            }

            return false;
        }
     

        public void OnInitializeFailed(InitializationFailureReason error) => 
            D.LogError($"Initialization failed error: {error}", this);

        public void OnInitializeFailed(InitializationFailureReason error, string message) => 
            D.LogError($"Initialization failed error: {error} message: {message}", this);

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            D.LogInfo($"ProcessPurchase {purchaseEvent.purchasedProduct.definition.id}", this);
            PurchaseRequest?.MarkedAsPurchased();
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            D.LogError($"PurchaseFailed " +
                           $"product: {product}" +
                           $"reason: {failureReason}", this);
            
            PurchaseRequest?.MarkAsFailed(failureReason.ToString());
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            D.LogInfo($"Initialized", this);
            Debug.Assert(productSettings != null);
            storeController = controller;
            storeExtensionProvider = extensions;
            registeredProducts = productSettings.Select(x =>
                new RuntimeProductItem(storeController.products.WithID(x.ProductId), x))
                .ToList();
            
            D.LogInfo(JsonConvert.SerializeObject(registeredProducts), this);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            D.LogError($"PurchaseFailed " +
                           $"product: {product}" +
                           $"reason: {failureDescription.reason} " +
                           $"description: {failureDescription.message}", this);
            
            PurchaseRequest?.MarkAsFailed(failureDescription.message);
        }

        private bool IsInitialized() => 
            storeController != null && storeExtensionProvider != null;
    }
}