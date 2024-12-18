using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OneDay.Core.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneDay.Core.Modules.Ui.Components
{
    public class CToggle : MonoBehaviour, IBeginDragHandler, IEndDragHandler
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

        private bool isAlreadySet;
        private bool isDragging = false;
        
        private void Awake()
        {
            if (toggleGroup == null)
            {
                toggleGroup = GetComponentInParent<CToggleGroup>();
            }
            if (toggleGroup != null)
                toggleGroup.RegisterToggle(this);

            button.onClick.AddListener(OnClick);

            if (!isAlreadySet)
            {
                isOn = false;
                TryToSwitchGameObjects(isOn, 0);
                TryToSwitchSprites(isOn);
                isAlreadySet = true;
            }
        }

        public void SetOn(bool isOn, bool invokeListeners)
        {
            if (isAlreadySet && this.isOn == isOn)
                return;
            
            isAlreadySet = true;
            this.isOn = isOn;

            TryToSwitchGameObjects(this.isOn, useFade ? fadeDuration : 0.0f);
            TryToSwitchSprites(this.isOn);
           
            if (invokeListeners)
            {
                onChange.Invoke(this.isOn);
            }
        }

        private void OnClick()
        {
            if (isDragging)
                return;
            
            if (toggleGroup != null && toggleGroup.IsUsingSwitchBehaviour)
            {
                if (!isOn)
                {
                    SetOn(true, true);
                    toggleGroup.Toggles
                        .Where(x=>x!=this)
                        .ForEach(x=>x.SetOn(false, true));   
                } else if (toggleGroup.MinSelectedToggles == 0)
                {
                    SetOn(false, true);
                }
            }
            else
            {
                if (toggleGroup != null && !toggleGroup.CanSetToggle(!isOn))
                    return;
                
                SetOn(!isOn, true);
            }
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

        private void TryToSwitchGameObjects(bool state, float duration)
        {
            if (onGameObject == null || offGameObject == null) return;

            if (ctx != null)
            {
                ctx.Cancel();
                ctx.Dispose();
            }

            ctx = new CancellationTokenSource();
            
            onGameObject.SetVisibleWithFade(state, duration, false, ctx.Token).Forget();
            offGameObject.SetVisibleWithFade(!state, duration, false, ctx.Token).Forget();
            
            //button.image.SetAlpha(0);
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

        public void OnBeginDrag(PointerEventData eventData) => isDragging = true;
        public void OnEndDrag(PointerEventData eventData) => isDragging = false;
    }
}