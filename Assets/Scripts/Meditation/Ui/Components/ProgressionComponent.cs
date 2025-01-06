using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class ProgressionComponent : MonoBehaviour
    {
        [SerializeField] private Image progressionImage;

        public void Set(int current, int total)
        {
            progressionImage.fillAmount = (float)current / total;
        }
    }
}