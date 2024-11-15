using System;
using System.Collections.Generic;
using Meditation.Data;
using Meditation.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class MoodView : UiView
    {
        public Button StartButton => startButton;
        
        [SerializeField] private Transform container;
        [SerializeField] private Button startButton;

        protected List<Mood> moods;

        private Action<int, bool> moodSelectionChanged;
        private IMoodDb moodDb;

        private int selectedMoods;
        protected override void OnInit()
        {
            Cg.alpha = 0;
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

        public void ResetMoods()
        {
            selectedMoods = 0;
            moods.ForEach(x=>x.GetComponent<CToggle>().SetOn(false, false));
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
                }
            }
            else
            {
                selectedMoods--;
            }
        }
    }
}