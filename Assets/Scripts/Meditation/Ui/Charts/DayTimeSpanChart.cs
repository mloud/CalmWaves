using System;
using System.Collections.Generic;
using Meditation.Core;
using TMPro;
using UnityEngine;

namespace Meditation.Ui.Chart
{
    public class DayTimeSpanChartData : IChartData<DayOfWeek, TimeSpan>
    {
        public TimeSpan MaxValue { get; set; }

        public List<(DayOfWeek xValue, TimeSpan yValue)> Values { get; set; } = new();
    }
    
    public class DayTimeSpanChart : MonoBehaviour, IChart<DayOfWeek, TimeSpan>
    {
        
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI selectionName;
        [SerializeField] private TextMeshProUGUI selectionValue;
        
        [SerializeField] private TextMeshProUGUI unitsLabel;

        [SerializeField] private DayTimeSpanChartColumn columnPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private List<DayTimeSpanChartColumn> columns;

        private void Awake()
        {
            columns.ForEach(x=>x.Button.onClick.AddListener(() =>
            {
                Select(x.ColumnName);
            }));
        }


        private IChartData<DayOfWeek, TimeSpan> data;
   
        public string Name
        {
            get => nameLabel.text;
            set => nameLabel.text = value;
        }

        public string Units
        {
            get => unitsLabel.text;
            set => unitsLabel.text = value;
        }

        public void Select(DayOfWeek selectedDay)
        {
            columns.ForEach(x=>x.SetSelected(x.ColumnName == selectedDay));
            selectionName.text = DateTimeUtils.GetLocalizedDayName(selectedDay);
            selectionValue.text = ((int)data.Values.Find(x => x.xValue == selectedDay)
                .yValue.TotalSeconds).ToString();
        }

        public void Set(IChartData<DayOfWeek, TimeSpan> data)
        {
            this.data = data;
            for (var index = 0; index < data.Values.Count; index++)
            {
                var columnData = data.Values[index];
                var column = columns[index];
                column.ColumnName = columnData.xValue;
                column.ColumnValue = columnData.yValue;
                column.NormalizedValue = data.MaxValue.TotalMilliseconds > 0
                    ? columnData.yValue.Milliseconds / (float)data.MaxValue.TotalMilliseconds
                    : 0;
            }
        }
    }
}