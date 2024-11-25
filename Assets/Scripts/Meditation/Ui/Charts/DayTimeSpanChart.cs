using System;
using System.Collections.Generic;
using System.Linq;
using Meditation.Core;
using Meditation.Core.Utils;
using TMPro;
using UnityEngine;

namespace Meditation.Ui.Chart
{
    public class DayTimeSpanChartData : IChartData<DayOfWeek, TimeSpan>
    {
        public TimeSpan MaxValue { get; set; }

        public List<(DayOfWeek xValue, TimeSpan yValue)> Values { get; set; }
        public DayTimeSpanChartData(IReadOnlyList<(DayOfWeek, TimeSpan)> days, TimeSpan maxValue)
        {
            Values = days.ToList();
            MaxValue = maxValue;
        }
        public DayTimeSpanChartData()
        {
            Values = new List<(DayOfWeek xValue, TimeSpan yValue)>();
        }  
    }
    
    public class DayTimeSpanChart : MonoBehaviour, IChart<DayOfWeek, TimeSpan>
    {
        public Func<TimeSpan, string> ValueToStringConversion { get; set; }
        
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
            selectionValue.text = ValueToStringConversion(
                data.Values.Find(x => x.xValue == selectedDay).yValue);
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
                    ? (float)columnData.yValue.TotalMilliseconds / (float)data.MaxValue.TotalMilliseconds
                    : 0;
            }
        }
    }
}