using System;
using TMPro;
using UnityEngine;

namespace OneDay.Core.Modules.Ui.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class TimeSpanText : MonoBehaviour
    {
        [SerializeField] private TimeFormat timeFormat = TimeFormat.HH_MM_SS;
        [SerializeField] private string customFormat = "yyyy/MM/dd HH:mm:ss"; // Custom format string
     
        // Optional suffixes for D_H_M_S format
        [SerializeField] private string daySuffix = " days";
        [SerializeField] private string hourSuffix = " hours";
        [SerializeField] private string minuteSuffix = " minutes";
        [SerializeField] private string secondSuffix = " seconds";

        [SerializeField] private TMP_Text tmpText;

        private void Awake()
        {
            if (tmpText == null)
                tmpText = GetComponent<TMP_Text>();
        }

        private void OnValidate()
        {
            if (tmpText == null)
            {
                tmpText = GetComponent<TMP_Text>();
            }
        }

        public enum TimeFormat
        {
            MM_SS, // Example: 30:15
            HH_MM_SS, // Example: 14:30:15
            HH_MM,
            DD_MM_YYYY, // Example: 26/11/2024
            MM_DD_YYYY, // Example: 11/26/2024
            YYYY_MM_DD, // Example: 2024-11-26
            D_H_M_S, // Days, Hours, Minutes, Seconds
            Custom // Custom user-defined format
        }

        public void Set(TimeSpan timeSpan) => tmpText.text = GetFormattedTimestamp(timeSpan);
        public void Set(DateTime dateTime) => tmpText.text = GetFormattedTimestamp(dateTime.TimeOfDay);

        private string GetFormattedTimestamp(TimeSpan timeSpan)
        {
            return timeFormat switch
            {
                TimeFormat.HH_MM_SS => timeSpan.ToString(@"hh\:mm\:ss"),
                TimeFormat.HH_MM => timeSpan.ToString(@"hh\:mm"),
                TimeFormat.DD_MM_YYYY => timeSpan.ToString(@"dd\:mm\:yyyy"),
                TimeFormat.MM_DD_YYYY => timeSpan.ToString(@"mm\:dd\:yyyy"),
                TimeFormat.YYYY_MM_DD => timeSpan.ToString(@"yyyy\:mm\:dd"),
                
                TimeFormat.D_H_M_S => GetTimeWithSuffixes(timeSpan),
                TimeFormat.Custom => timeSpan.ToString(customFormat),
                _ => timeSpan.ToString()
            };
        }

        private string GetTimeWithSuffixes(TimeSpan timeSpan)
        {
            // Example: Calculate total time in custom breakdown
            int days = timeSpan.Days;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            // Concatenate the optional suffixes
            string formattedTime =
                $"{days}{daySuffix}, {hours}{hourSuffix}, {minutes}{minuteSuffix}, {seconds}{secondSuffix}";
            return formattedTime;
        }
    }
}