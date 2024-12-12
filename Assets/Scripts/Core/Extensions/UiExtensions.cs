using Cysharp.Threading.Tasks;
using DG.Tweening;
using OneDay.Core.Modules.Localization;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneDay.Core.Extensions
{
    public static class UiExtensions
    {
        public static void SetAlpha(this Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
        
        public static void SetAlpha(this TMP_Text text, float alpha)
        {
            var color = text.color;
            color.a = alpha;
            text.color = color;
        }
        
        public static void SetTextId(this TMP_Text label, string textId) => 
            label.text = ServiceLocator.Get<ILocalizationManager>().Localize(textId);

        public static void SetTextId(this AExtendedText label, string textId) => 
            label.text = ServiceLocator.Get<ILocalizationManager>().Localize(textId);


        public static void SetFrom(this RectTransform rectTransform, RectTransform source)
        {
            rectTransform.anchorMax = source.anchorMax;
            rectTransform.anchorMin = source.anchorMin;
            rectTransform.offsetMax = source.offsetMax;
            rectTransform.offsetMin = source.offsetMin;
        }
        
        public static async UniTask SetFromAsync(this RectTransform rectTransform, RectTransform source, float duration, Ease ease)
        {
            await UniTask.WhenAll(
                DOTween.To(() => rectTransform.offsetMin, v => rectTransform.offsetMin = v, source.offsetMin, duration).SetEase(ease)
                    .ToUniTask(),
                DOTween.To(() => rectTransform.offsetMax, v => rectTransform.offsetMax = v, source.offsetMax, duration).SetEase(ease)
                    .ToUniTask()
            );

              
          //  rectTransform.offsetMin = source.offsetMin;
          //  rectTransform.offsetMax = source.offsetMax;
            
            
            await UniTask.WhenAll(
                rectTransform.DOAnchorMin(source.anchorMin, duration).SetEase(ease).ToUniTask(),
                rectTransform.DOAnchorMax(source.anchorMax, duration).SetEase(ease).ToUniTask());
        }
    }
}