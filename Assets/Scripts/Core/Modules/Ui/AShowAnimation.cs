using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace OneDay.Core.Modules.Ui
{
    public abstract class AShowAnimation : MonoBehaviour
    {
        [SerializeField] protected float duration = 0.5f;
        [SerializeField] protected Ease ease;
        public abstract UniTask Show(bool useSmooth, float speedMultiplier = 1.0f);
        public abstract UniTask Hide(bool useSmooth, float speedMultiplier = 1.0f);
    }
}