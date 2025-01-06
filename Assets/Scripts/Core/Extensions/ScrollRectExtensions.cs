using System;
using UnityEngine;
using UnityEngine.UI;

namespace OneDay.Core.Extensions
{
    public static class ScrollRectExtensions
    {
        public static void CenterOnItem(this ScrollRect scrollRect, RectTransform targetItem, RectTransform centerTransform)
        {
            if (targetItem == null)
                throw new ArgumentException("Target item is null!");

            if (targetItem.parent != scrollRect.content)
                throw new ArgumentException("Target item must be parent of scroll rect content");

            var offset = centerTransform.position - targetItem.position;

            scrollRect.content.position += offset;
        }
    }
}