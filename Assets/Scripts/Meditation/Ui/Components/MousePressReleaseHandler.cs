using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Meditation.Ui.Components
{
    public class MousePressReleaseHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent onPointerDown;
        public UnityEvent onPointerUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp.Invoke();
        }
    }
}