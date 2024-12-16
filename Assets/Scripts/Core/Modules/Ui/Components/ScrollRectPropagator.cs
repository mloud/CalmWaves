using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDay.Core.Modules.Ui.Components
{
    public class ScrollRectPropagator : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private ScrollRect parentScrollRect;
        [SerializeField] private ScrollRect thisScrollRect;

        private void Awake()
        {
            parentScrollRect = GetComponentInParent<ScrollRect>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsProperlySetup())
                return;
            
            // Check drag direction and decide if the parent scroll rect should handle it
            if (CouldScrollThis(eventData))
            {
                thisScrollRect.OnBeginDrag(eventData);
            }
            else
            {
                parentScrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsProperlySetup())
                return;
            
            if (CouldScrollThis(eventData))
            {
                thisScrollRect.OnDrag(eventData);
            }
            else
            {
                parentScrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsProperlySetup())
                return;
  
            if (CouldScrollThis(eventData))
            {
                thisScrollRect.OnEndDrag(eventData);
            }
            else
            {
                parentScrollRect.OnEndDrag(eventData);
            }
        }

        private bool IsProperlySetup() => parentScrollRect != null;

        private bool CouldScrollThis(PointerEventData eventData)
        {
            if (thisScrollRect == null)
                return false;
            
            if (thisScrollRect.horizontal && Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
                return true;
            
            if (thisScrollRect.vertical && Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x))
                return true;

            return false;
        }
    }
}