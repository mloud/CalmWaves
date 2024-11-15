using System.Globalization;
using DG.Tweening;
using Meditation.Apis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Timer
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeLabel;
        [SerializeField] private CanvasGroup buttonsCg;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        [SerializeField] private float autoHideTime;
        [SerializeField] private float animationDuration = 0.5f;
        
        private IBreathingApi breathingApi;
        private Sequence sequence;
        private float autoHideTimer;

        private Vector3 leftButtonOriginalPosition;
        private Vector3 rightButtonOriginalPosition;
        private Vector3 leftButtonHiddenPosition;
        private Vector3 rightButtoHiddenPosition;

        public void Initialize()
        {
            breathingApi = ServiceLocator.Get<IBreathingApi>();
            autoHideTimer = -1;
            buttonsCg.interactable = false;
            buttonsCg.alpha = 0;
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
            leftButtonOriginalPosition = leftButton.transform.localPosition;
            leftButtonHiddenPosition = leftButtonOriginalPosition + new Vector3(50, 0, 0);
            rightButtonOriginalPosition = rightButton.transform.localPosition;
            rightButtoHiddenPosition = rightButtonOriginalPosition - new Vector3(50, 0, 0);
            leftButton.transform.localPosition = leftButtonHiddenPosition;
            rightButton.transform.localPosition = rightButtoHiddenPosition;

            Refresh();
        }

        public void ShowButtons()
        {
            sequence?.Kill();
            sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                leftButton.gameObject.SetActive(true);
                rightButton.gameObject.SetActive(true);
            });
            sequence.Append(buttonsCg.DOFade(1, animationDuration).SetEase(Ease.Linear));
            sequence.Join(leftButton.transform.DOLocalMoveX(leftButtonOriginalPosition.x, animationDuration)
                .From(Mathf.Min(leftButtonHiddenPosition.x, leftButton.transform.localPosition.x))
                .SetEase(Ease.Linear));
            sequence.Join(rightButton.transform.DOLocalMoveX(rightButtonOriginalPosition.x, animationDuration)
                .From(Mathf.Max(rightButtoHiddenPosition.x, rightButton.transform.localPosition.x))
                .SetEase(Ease.Linear));

            sequence.AppendCallback(()=>buttonsCg.interactable = true);
            sequence.AppendCallback(SetAutoHideToMax);
        }

        private void HideButtons()
        {
            sequence?.Kill();
            sequence = DOTween.Sequence();
            sequence.AppendCallback(() => autoHideTimer = -1);
            sequence.AppendCallback(()=>buttonsCg.interactable = false);
            sequence.Append(buttonsCg.DOFade(0, animationDuration).SetEase(Ease.Linear));
            sequence.Join(leftButton.transform.DOLocalMoveX(leftButtonHiddenPosition.x, animationDuration)
                .From(Mathf.Max(leftButtonOriginalPosition.x, leftButton.transform.localPosition.x))
                .SetEase(Ease.Linear));
            sequence.Join(rightButton.transform.DOLocalMoveX(rightButtoHiddenPosition.x, animationDuration)
                .From(Mathf.Min(rightButtonOriginalPosition.x, rightButton.transform.localPosition.x))
                .SetEase(Ease.Linear));
            sequence.AppendCallback(() =>
            {
                leftButton.gameObject.SetActive(false);
                rightButton.gameObject.SetActive(false);
            });
        }

        private void Update()
        {
            if (autoHideTimer > 0)
            {
                autoHideTimer -= Time.deltaTime;
                if (autoHideTimer < 0)
                {
                    HideButtons();
                }
            }
        }
        
        public void Increase()
        {
            breathingApi.IncreaseBreathingTime();
            Refresh();
            SetAutoHideToMax();
        }

        public void Decrease()
        {
            breathingApi.DecreaseBreathingTime();
            Refresh();
            SetAutoHideToMax();
        }

        private void Refresh()
        {
            timeLabel.text =
                $"{breathingApi.GetBreathingTime().TotalMinutes.ToString(CultureInfo.InvariantCulture)} min";
        }

        private void SetAutoHideToMax() => autoHideTimer = autoHideTime;
    }
}