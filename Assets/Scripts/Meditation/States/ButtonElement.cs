using Meditation.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.States
{
    [RequireComponent(typeof(Button))]
    public class ButtonElement : UiElement
    {
        public Button ButtonRef => button;
        
        [SerializeField] private Button button;
        private void OnValidate()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
                if (button == null)
                {
                    Debug.LogError("No Button found");
                }
            }
        }
    }
}