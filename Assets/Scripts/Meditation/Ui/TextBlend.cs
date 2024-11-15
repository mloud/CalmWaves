using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Meditation.Ui
{
    public class TextBlend :MonoBehaviour
    {
        [SerializeField] private float duration;
        [SerializeField] private TextMeshProUGUI text1;
        [SerializeField] private TextMeshProUGUI text2;

        private TextMeshProUGUI actualText;
        
        public async UniTask SetNewText(string text)
        {
            if (actualText == null)
            {
                actualText = text1;
                actualText.text = text;
                actualText.DOFade(1, duration);
                text2.text = "";
                return;
            }

            var oldText = actualText;
            var newText = actualText == text1 ? text2 : text1;
            newText.text = text;
            oldText.DOFade(0, duration);
            newText.DOFade(1, duration);
            actualText = newText;
            await UniTask.WaitForSeconds(duration);
        }
    }
}