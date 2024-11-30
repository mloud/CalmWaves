using System;
using System.Collections.Generic;
using System.Threading;
using Core.Modules.Ui.Effects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Data;
using Meditation.Ui.Components;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class MoodView : UiView
    {
        public GenerateButton GenerateButton => generateButton;

        [SerializeField] private CanvasGroup moodeCg;
        [SerializeField] private Transform container;
        [SerializeField] private Button startButton;
        [SerializeField] private GenerateButton generateButton;

        private List<Mood> moods;
        private Action<int, bool> moodSelectionChanged;
        private int selectedMoods;

        public void Initialize(IMoodDb moodDb, Mood moodPrefab, Action<int, bool> moodSelectionChanged)
        {
            this.moodSelectionChanged = moodSelectionChanged;
            
            moods = new List<Mood>();
            int index = 0;
            foreach (var mood in moodDb.Moods)
            {
                var moodButton = Instantiate(moodPrefab, container);
                moodButton.Set(mood);
            
                int currentIndex = index;
                moodButton.GetComponent<CToggle>()
                    .onChange
                    .AddListener(selected=>OnSelected(currentIndex, selected));
                moods.Add(moodButton);
                index++;
            }
        }
     
        public async UniTask Animate(bool animateToVisible, CancellationToken token)
        {
            for (int i = 0; i < moods.Count; i++)
            {
                moods[i].gameObject.SetVisibleWithFade(animateToVisible, 2.0f, true, token).Forget();
                await UniTask.WaitForSeconds(0.05f, cancellationToken: token);
                moods[i].GetComponent<SpringMoveTween>().Target = container;
                moods[i].GetComponent<SpringMoveTween>().Run();
            }
         
        }
     
        public void Reset()
        {
            selectedMoods = 0;
            moods.ForEach(x=>x.GetComponent<CToggle>().SetOn(false, false));
            moods.ForEach(x=>x.gameObject.SetVisibleWithFade(false, 0, true).Forget());
            generateButton.Reset();
        }

        public async UniTask SwitchToGenerateMode()
        {
            await UniTask.WhenAll(
                moodeCg.DOFade(0, 1.0f).SetEase(Ease.Linear).ToUniTask(),
                generateButton.AnimateToGenerating());
        }
        
        private void OnSelected(int index, bool isSelected)
        {
            selectedMoods += isSelected ? 1 : -1;
            moodSelectionChanged(index, isSelected);

            if (selectedMoods == 0) 
                generateButton.SetButtonActive(false).Forget();
            else 
                generateButton.SetButtonActive(true).Forget();
        }
    }
}