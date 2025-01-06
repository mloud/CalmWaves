using TMPro;
using UnityEngine;

namespace Meditation.Ui.Components
{
    public class ChapterInfoComponent : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text subtitleText;

        public void Set(string title, string subtitle)
        {
            titleText.text = title;
            subtitleText.text = subtitle;
        }
    }
}