using System;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public abstract class AValueChanger<T> : MonoBehaviour where T: IComparable<T>
    {
        public T Value => value;
        public Action<T> OnValueChanged { get; set; }
        
        [SerializeField] private Button plusButton;
        [SerializeField] private Button minusButton;
        [SerializeField] private AExtendedText label;

        private T value;
        private T minValue;
        private T maxValue;

        private void Awake()
        {
            plusButton.onClick.AddListener(OnIncreaseValue);
            minusButton.onClick.AddListener(OnDecreaseValue);
        }

        public void Initialize(T value, T minValue, T maxValue)
        {
            SetButtonsVisible(true, false);
            this.minValue = minValue;
            this.maxValue = maxValue;
            Set(value);
        }

        public void Set(T value, bool callListeners = false)
        {
            this.value = ClampValue(value);
            label.text = value.ToString();
            
            if (callListeners)
                OnValueChanged?.Invoke(this.value);
        }

        public void SetButtonsVisible(bool isVisible, bool useFade)
        {
            plusButton.gameObject.SetVisibleWithFade(isVisible, useFade ? 0.5f :0, true);
            minusButton.gameObject.SetVisibleWithFade(isVisible, useFade ? 0.5f :0, true);
        }
        
        protected abstract T IncreaseValue(in T value);
        protected abstract T DecreaseValue(in T value);
        
        private T ClampValue(T value)
        {
            if (value.CompareTo(minValue) < 0)
                return minValue;
         
            if (value.CompareTo(maxValue) > 0)
                return maxValue;
            
            return value;
        }

        private void OnIncreaseValue()
        {
            value = ClampValue(IncreaseValue(value));
            label.text = value.ToString();
            OnValueChanged?.Invoke(value);
        }

        private void OnDecreaseValue()
        {
            value = ClampValue(DecreaseValue(value));
            label.text = value.ToString();
            OnValueChanged?.Invoke(value);
        }
    }
}