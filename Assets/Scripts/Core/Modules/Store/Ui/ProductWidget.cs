using OneDay.Core.Modules.Store.Data;
using TMPro;
using UnityEngine;

namespace OneDay.Core.Modules.Store.Ui
{
    public class ProductWidget :MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI descriptionLabel;
        [SerializeField] private TextMeshProUGUI priceLabel;

        public void Set(RuntimeProductItem runtimeProductItem)
        {
            nameLabel.text = runtimeProductItem.Setting.Name;
            if (descriptionLabel != null)
            {
                descriptionLabel.text = runtimeProductItem.Setting.Description;
            }

            priceLabel.text = runtimeProductItem.Product.metadata.localizedPriceString;
        }
    }
}