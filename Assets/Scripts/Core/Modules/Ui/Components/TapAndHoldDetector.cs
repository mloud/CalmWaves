using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OneDay.Core.Modules.Ui.Components
{
    public class TapAndHoldDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public UnityEvent onLongPress;
        public UnityEvent onClick;
        
        [SerializeField] float holdTimeThreshold = 2.0f;

        private bool isHolding;
        private bool longPressTriggered;
        private float holdTime;

        private void Update()
        {
            if (isHolding)
            {
                holdTime += Time.deltaTime;

                if (holdTime >= holdTimeThreshold)
                {
                    longPressTriggered = true; // Mark that a long press occurred
                    isHolding = false;
                    onLongPress?.Invoke();
                    Debug.Log("Long Press Detected");
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            holdTime = 0f;
            longPressTriggered = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
            holdTime = 0f;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!longPressTriggered)
            {
                Debug.Log("Button click ignored due to long press.");
                onClick.Invoke();
            }
        }
    }
}