using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Chart
{
    public class DayTimeSpanChartColumn : MonoBehaviour
    {
        public Button Button => button;
        
        [SerializeField] private TextMeshProUGUI nameColumn;
        [SerializeField] private TextMeshProUGUI valueColumn;
        [SerializeField] private Image columnImage;
        [SerializeField] private GameObject selection;
        [SerializeField] private GameObject zeroValue;
        [SerializeField] private Button button;
        
        private TimeSpan value;
        private DayOfWeek name;
        private float normalizedValue;

        
        public DayOfWeek ColumnName
        {
            get => name;
            set
            {
                nameColumn.text = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(value)[..2];
                name = value;
            }
        }

        public void SetSelected(bool isSelected) => selection.SetActive(isSelected);
        
        public TimeSpan ColumnValue
        {
            get => value;
            set
            {
                valueColumn.text = $"{(int)value.TotalMinutes}:{value.Seconds:D2}s";
                this.value = value;
            }
        }

        public float NormalizedValue
        {
            get => normalizedValue;
            set
            {
                normalizedValue = value;
                columnImage.gameObject.SetScaleY(value);
                zeroValue.SetActive(value == 0);
            }
        }
    }
}