using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Meditation.Ui.Audio
{
    public class AudioVolumeChanger : MonoBehaviour, IDragHandler
    {
        [SerializeField] private Image fillImage;
        
        public Action<float> VolumeChange { get; set; }
    
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        public void SetVolume(float volume) => fillImage.fillAmount = volume;
        
        public void OnDrag(PointerEventData eventData)
        {
            if (Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y))
                return;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, 
                eventData.position, 
                eventData.pressEventCamera, 
                out var localPoint
            );
        
            //float width = rectTransform.rect.width;
            //float n = (localPoint.x - rectTransform.rect.xMin) / width;
            //fillImage.fillAmount = n;

            fillImage.fillAmount += eventData.delta.x / rectTransform.rect.width;
            VolumeChange?.Invoke(fillImage.fillAmount);
        }

        public void SetVisible(bool visible)
        {
            fillImage.fillAmount = 0.5f;
            fillImage.enabled = visible;
        }
    }
}