using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDay.Core.Modules.Ui.Components
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AutoDimComponent : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float dimedAlpha = 0.1f;
        [SerializeField] private float visibleTime = 3.0f;
        [SerializeField] private float fadeSpeed = 2f;
     
        private float visibilityTimer;
        private void OnEnable()
        {
            visibilityTimer = visibleTime;    
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            visibleTime = visibilityTimer;
        }

        private void Update()
        {
            if (visibilityTimer > 0)
            {
                if (canvasGroup.alpha < 1)
                {
                    float alphaStep = fadeSpeed * Time.deltaTime;
                    float alpha = canvasGroup.alpha;
                    alpha += alphaStep;
                    canvasGroup.alpha = Mathf.Clamp01(alpha);
                }
                visibilityTimer -= Time.deltaTime;
            }
            else
            {
                if (canvasGroup.alpha > dimedAlpha)
                {
                    float alphaStep = fadeSpeed * Time.deltaTime;
                    float alpha = canvasGroup.alpha;
                    alpha -= alphaStep;
                    canvasGroup.alpha = Mathf.Clamp01(alpha);
                }
            }
        }
    }
}