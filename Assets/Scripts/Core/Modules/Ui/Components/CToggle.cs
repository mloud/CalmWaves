using System.Threading;
using Cysharp.Threading.Tasks;
using OneDay.Core.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OneDay.Core.Modules.Ui.Components
{
    public class CToggle : MonoBehaviour
    {
        public bool IsOn => isOn;
        public UnityEvent<bool> onChange;
       
        [SerializeField] private Button button;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private GameObject onGameObject;
        [SerializeField] private GameObject offGameObject;
        [SerializeField] private bool useFade;
        [SerializeField] private CToggleGroup toggleGroup;

        private CancellationTokenSource ctx;
        private bool isOn;
        private float fadeDuration = 0.2f;
        private void Awake()
        {
            if (toggleGroup == null)
            {
                toggleGroup = GetComponentInParent<CToggleGroup>();
            }
            if (toggleGroup != null)
                toggleGroup.RegisterToggle(this);

            button.onClick.AddListener(OnClick);
            SetOn(false, false);
            
        }

        public void SetOn(bool isOn, bool invokeListeners)
        {
            if (toggleGroup != null && !toggleGroup.CanSetToggle(isOn))
                return;
            
            this.isOn = isOn;
            
            TryToSwitchGameObjects(this.isOn);
            TryToSwitchSprites(this.isOn);
           
            if (invokeListeners)
            {
                onChange.Invoke(this.isOn);
            }
        }

        private void OnClick()
        {
            SetOn(!isOn, true);
        }

        private void OnValidate()
        {
            button = GetComponent<Button>();
        }

        private void OnDestroy()
        {
            if (toggleGroup != null)
                toggleGroup.UnregisterToggle(this);
        }

        private void TryToSwitchGameObjects(bool state)
        {
            if (onGameObject == null || offGameObject == null) return;

            if (ctx != null)
            {
                ctx.Cancel();
                ctx.Dispose();
            }

            ctx = new CancellationTokenSource();
            
            onGameObject.SetVisibleWithFade(state, useFade ? fadeDuration : 0, false, ctx.Token).Forget();
            offGameObject.SetVisibleWithFade(!state, useFade ? fadeDuration : 0, false, ctx.Token).Forget();
            
            button.image.SetAlpha(0);
        }

        private void TryToSwitchSprites(bool state)
        {
            if (onSprite == null || offSprite == null) return;
            if (onGameObject != null)
                onGameObject.SetActive(false);

            if (offGameObject != null)
                offGameObject.SetActive(false);
       
            button.image.SetAlpha(1.0f);
            button.image.sprite = state ? onSprite : offSprite;
        }
    }
}