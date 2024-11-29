using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OneDay.Core.Extensions;
using UnityEngine;

namespace Meditation.Ui.Components
{
    public class ExpandableArea : MonoBehaviour
    {
        public Action<bool> OnExpanded { get; set; }
        
        [SerializeField] private RectTransform transformToExpand;
        [SerializeField] private RectTransform expandedArea;
        [SerializeField] private RectTransform collapsedArea;

        public void SetExpanded(bool isExpanded)
        {
            if (isExpanded)
            {
                transformToExpand.SetFromAsync(expandedArea, 0.5f, Ease.InOutSine).Forget();
            }
            else
            {
                transformToExpand.SetFromAsync(collapsedArea, 0.5f, Ease.InOutSine).Forget();
            }
            OnExpanded?.Invoke(isExpanded);
        }
    }
}