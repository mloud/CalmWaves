using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meditation.Data;
using Meditation.Ui.Components;
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
        private IMoodDb moodDb;
        private int selectedMoods;

        protected override void OnInit()
        {
            base.OnInit();
            LookUp.Get<MoodView>().Register(this);
        }
       
        public void Initialize(IMoodDb moodDb, Mood moodPrefab, Action<int, bool> moodSelectionChanged)
        {
            this.moodDb = moodDb;
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

        public void Reset()
        {
            selectedMoods = 0;
            moodeCg.alpha = 1;
            moods.ForEach(x=>x.GetComponent<CToggle>().SetOn(false, false));
            generateButton.Reset();
        }

        public async UniTask SwitchToGenerateMode()
        {
            await UniTask.WhenAll(
                moodeCg.DOFade(0, 1.0f).SetEase(Ease.Linear).ToUniTask(),
                generateButton.AnimateToGenerating(1.0f));
        }
        
        private void OnSelected(int index, bool isSelected)
        {
            if (isSelected)
            {
                if (selectedMoods >= moodDb.MaxMoodsSelected)
                {
                    moods[index].GetComponent<CToggle>().SetOn(false, false);
                }
                else
                {
                    selectedMoods++;
                    moodSelectionChanged(index, true);
                }
            }
            else
            {
                selectedMoods--;
                moodSelectionChanged(index, false);
            }
        }
    }
}