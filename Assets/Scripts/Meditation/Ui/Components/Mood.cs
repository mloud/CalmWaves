using TMPro;
using UnityEngine;

namespace Meditation.Ui.Components
{
    public class Mood : MonoBehaviour
    { 
        [SerializeField] private TextMeshProUGUI moodLabel;
        
        public void Set(string mood) => moodLabel.text = mood;
    }
}