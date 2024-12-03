using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace OneDay.Core.Modules.Ui.Components
{
    [RequireComponent(typeof(Image))]
    public class CImage : MonoBehaviour
    {
        private AddressableAsset<Sprite> asset;

        [SerializeField] private Image image;
        private void Awake()
        {
            if (image == null)
                image = GetComponent<Image>();
        }

        private void OnValidate()
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }
        }

        public async UniTask SetImage(string id)
        {
            if (asset != null && asset.GetReference().name == id)
                return;

            asset?.Release();
            asset = null;  
            
            asset = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<Sprite>(id);
            if (asset != null)
            {
                image.sprite = asset.GetReference();
            }
            else
            {
                Debug.LogError($"Could not load sprite with id {id}");
            }
        }

        private void OnDestroy()
        {
            //asset?.Release();
        }
    }
}