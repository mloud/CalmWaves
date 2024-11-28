using OneDay.Core.Modules.Localization;
using OneDay.Core.Modules.Ui.Components;
using TMPro;
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
    }
}