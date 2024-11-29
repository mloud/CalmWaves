using OneDay.Core.Modules.Vibrations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OneDay.Core.Modules.Ui.Components
{
    public class TapAndHoldDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public UnityEvent onLongPress;
        public UnityEvent onClick;
        
        [SerializeField] private float holdTimeThreshold = 2.0f;
        [SerializeField] private bool useVibration = true;
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
                    ServiceLocator.Get<IVibrationManager>().VibrateTiny();
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