using UnityEngine;
using UnityEngine.UI;

namespace OneDay.Core.Modules.Ui.Components
{
    public class ScrollRectSnap : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect; // Reference to the ScrollRect component
       
        public void CenterOnItem(RectTransform targetItem)
        {
            if (scrollRect == null || targetItem == null) return;

            var targetPosition = (Vector2)scrollRect.content.InverseTransformPoint(targetItem.position);

            var viewport = scrollRect.viewport;
            var viewportSize = viewport.rect.size;

            var offset = new Vector2(
                targetPosition.x - (viewportSize.x / 2),
                targetPosition.y - (viewportSize.y / 2)
            );

            // Normalize the offset to a range of 0 to 1
            var normalizedPosition = new Vector2(
                Mathf.Clamp01(offset.x / (scrollRect.content.rect.width - viewportSize.x)),
                Mathf.Clamp01(offset.y / (scrollRect.content.rect.height - viewportSize.y))
            );

            // Set the ScrollRect's normalized position
            scrollRect.normalizedPosition = new Vector2(
                scrollRect.horizontal ? normalizedPosition.x : scrollRect.normalizedPosition.x,
                scrollRect.vertical ? 1f - normalizedPosition.y : scrollRect.normalizedPosition.y
            );
        }
    }
}