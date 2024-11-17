using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Tweens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class GenerateButton : MonoBehaviour
    {
        public Button Button => button;

        [SerializeField] private PulsateTween pulsateTween;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Transform originalContainer;
        [SerializeField] private Transform generatingContainer;
        [SerializeField] private CanvasGroup cg;
        public void Reset()
        {
            label.text = "Start";
            transform.position = originalContainer.position;
            pulsateTween.SpeedMultiplier = 1.0f;
        }

        public async UniTask AnimateToGenerating(float time)
        {
            await cg.DOFade(0, time).SetEase(Ease.Linear).AsyncWaitForCompletion();
            transform.position = generatingContainer.position;
            label.text = "Generating";
            await cg.DOFade(1, time).SetEase(Ease.Linear).AsyncWaitForCompletion();
            pulsateTween.SpeedMultiplier = 10.0f;
        }
    }
}