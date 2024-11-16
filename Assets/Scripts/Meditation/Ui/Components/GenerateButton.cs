using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class GenerateButton : MonoBehaviour
    {
        public Button Button => button;
        
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Transform originalContainer;
        [SerializeField] private Transform generatingContainer;
        [SerializeField] private CanvasGroup cg;
        public void Reset()
        {
            label.text = "Start";
            transform.position = originalContainer.position;
        }

        public async UniTask AnimateToGenerating(float time)
        {
            await cg.DOFade(0, time).SetEase(Ease.Linear).AsyncWaitForCompletion();
            transform.position = generatingContainer.position;
            label.text = "Generating";
            await cg.DOFade(1, time).SetEase(Ease.Linear).AsyncWaitForCompletion();
        }
    }
}