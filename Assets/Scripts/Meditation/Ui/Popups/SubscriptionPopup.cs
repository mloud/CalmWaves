using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Store;
using OneDay.Core.Modules.Store.Ui;
using OneDay.Core.Modules.Ui;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class SubscriptionPopup : UiPopup
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private List<CToggle> subscritionDurationsToggles;
        [SerializeField] private ProductPanel productPanel;
            
        private bool isInitialized;
        
        protected void Awake()
        {
            continueButton.onClick.AddListener(()=>OnContinue());
        }

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            if (!isInitialized)
            {
                await InitializeSubscriptions();
                isInitialized = true;
            }
            productPanel.Get(1).GetComponent<CToggle>().SetOn(true, false);
            
            ServiceLocator.Get<IUiManager>().HideView();
        }

        protected override async UniTask OnCloseStarted()
        {
            ServiceLocator.Get<IUiManager>().ShowView();
        }

        private async UniTask InitializeSubscriptions()
        {
            var subscriptions = ServiceLocator.Get<IStoreManager>()
                .GetProducts(ProductType.Subscription)
                .ToList();

            productPanel.Prepare(subscriptions.Count);
            
            for (int i = 0; i < subscriptions.Count; i++)
            {
                productPanel.Get(i).Set(subscriptions[i]);         
            }
        }
        
        private async UniTask OnContinue()
        {
            var selectedProduct = productPanel.Items.FirstOrDefault(x => x.GetComponent<CToggle>().IsOn);
            Debug.Assert(selectedProduct!= null);
            Debug.Log($"Purchasing  {selectedProduct.ProductItem.Product.definition.id}");
            await ServiceLocator.Get<IStoreManager>().BuyProduct(selectedProduct.ProductItem.Product.definition.id);
        }
    }
}