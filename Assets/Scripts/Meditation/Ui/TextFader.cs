using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Meditation.Ui
{
    public class TextFader: MonoBehaviour
    {
        [SerializeField] private List<string> texts;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private float fadeDuration;
        [SerializeField] private float textDuration;
        [SerializeField] private bool kepLastTextVisible;

        private void Awake()
        {
            label.enabled = false;
        }

        public async UniTask Show()
        {
            DOTween.Kill(label);
            label.enabled = true;
            for (int i = 0; i < texts.Count; i++)
            {
                label.text = texts[i];
                await label.DOFade(1, fadeDuration).From(0).SetEase(Ease.Linear).AsyncWaitForCompletion();
                await UniTask.WaitForSeconds(textDuration);
                if (i < texts.Count - 1 && !kepLastTextVisible)
                {
                    await label.DOFade(0, fadeDuration).From(0).SetEase(Ease.Linear).AsyncWaitForCompletion();
                }
            }

            if (!kepLastTextVisible)
            {
                label.enabled = false;
            }
        }

        public async UniTask Hide()
        {
            await label.DOFade(0, fadeDuration).AsyncWaitForCompletion();
            Clear();
        }
        
        
        public void Clear()
        {
            label.text = "";
            label.enabled = false;
        }
    }
}