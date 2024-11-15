using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class CToggle : MonoBehaviour
    {
        public UnityEvent<bool> onChange;
       
        [SerializeField] private Button button;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;
    
        private bool isOn;
        private void Awake()
        {
            button.onClick.AddListener(OnClick);
            SetOn(false, false);
        }

        public void SetOn(bool isOn, bool invokeListeners)
        {
            this.isOn = isOn;
            button.image.sprite = this.isOn ? onSprite : offSprite;
            if (invokeListeners)
            {
                onChange.Invoke(this.isOn);
            }
        }

        private void OnClick()
        {
            SetOn(!isOn, true);
        }

        private void OnValidate()
        {
            button = GetComponent<Button>();
        }
    }
}