using TMPro;
using UnityEngine;

namespace Meditation.Ui
{
    public class LabelWithImage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        public void Set(string text) => label.text = text;
    }
}