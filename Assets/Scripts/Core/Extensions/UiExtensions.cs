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
    }
}