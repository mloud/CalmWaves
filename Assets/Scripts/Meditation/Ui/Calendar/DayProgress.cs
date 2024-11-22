using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Calendar
{
    public class DayProgress : MonoBehaviour
    {
        public DayOfWeek DayOfWeek => dayOfWeek;
        
        [SerializeField] private DayOfWeek dayOfWeek;
        [SerializeField] private TextMeshProUGUI dayLabel;
        [SerializeField] private Image progressImage;
        [SerializeField] private GameObject todayFlag;
        [SerializeField] private float actualizeDuration = 1.0f;
        
        private void Awake()
        {
            dayLabel.text = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dayOfWeek)[..2];
        }

        public void SetIsToday(bool isToday) => todayFlag.SetActive(isToday);
        
        public void Set(float currentDone, float total, bool isToday)
        {
            progressImage.fillAmount = (float)currentDone / total;
            SetIsToday(isToday);
        }
        
        public async UniTask Actualize(float currentDone, float total, bool isToday)
        {
            await progressImage.DOFillAmount(currentDone / total, actualizeDuration).AsyncWaitForCompletion();
            SetIsToday(isToday);
        }
    }
}