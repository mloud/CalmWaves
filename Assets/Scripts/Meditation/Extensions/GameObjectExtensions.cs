using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation
{
    public static class GameObjectExtensions
    {
        public static async UniTask SetVisibleWithFade(this Component component, bool isVisible, float duration,
            bool includeChildren)
        {
            await SetVisibleWithFade(component.gameObject, isVisible, duration, includeChildren);
        }
        
        public static async UniTask SetVisibleWithFade(this GameObject go, bool isVisible, float duration,
            bool includeChildren)
        {
            if (isVisible)
                go.SetActive(true);
            if (includeChildren)
            {
                var canvasGroup = go.GetComponent<CanvasGroup>();

                if (go.transform.childCount > 0)
                {
                    if (canvasGroup == null)
                    {
                        Debug.LogWarning(
                            $"GameObject {go.name} does not have CanvasGroup to fade its children - adding one");
                        canvasGroup = go.AddComponent<CanvasGroup>();
                    }
                }

                float targetAlpha = isVisible ? 1 : 0;
                if (canvasGroup != null)
                    await canvasGroup.DOFade(targetAlpha, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
                else
                {
                    var image = go.GetComponent<Image>();
                    if (image != null)
                        await image.DOFade(targetAlpha, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
                    else
                    {
                        var spriteRenderer = go.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null)
                            await spriteRenderer.DOFade(targetAlpha, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
                        else
                        {
                            var textUi = go.GetComponent<TextMeshProUGUI>();
                            if (textUi != null)
                                await textUi.DOFade(targetAlpha, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
                            else
                            {
                                var text = go.GetComponent<TextMeshPro>();
                                if (text != null)
                                    await text.DOFade(targetAlpha, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
                            }
                        }
                    }
                }
            }
            
            if (!isVisible)
                go.SetActive(false);
        }
    }
}