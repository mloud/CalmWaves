using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Data;
using Meditation.Ui;
using Meditation.Ui.Components;
using Meditation.Ui.Views;
using UnityEngine;

namespace Meditation.States
{
    public class MoodState : AState
    {
        private MoodView moodView;
        private AddressableAsset<GameObject> moodPrefab;
        private AddressableAsset<MoodDb> moodDbAsset;
        
        private IBreathingApi breathingApi;

        private HashSet<int> selectedMoods;
        
        public override async UniTask Initialize()
        {
            selectedMoods = new HashSet<int>();
            breathingApi = ServiceLocator.Get<IBreathingApi>();
            moodDbAsset = await ServiceLocator.Get<IDataManager>().GetMoodSettings();
            moodPrefab = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<GameObject>("MoodButton");
            moodView = LookUp.Get<MoodView>().GetFirst();
            moodView.Initialize(moodDbAsset.GetReference(), moodPrefab.GetReference().GetComponent<Mood>(), OnMoodSelectionChanged);
            moodView
                .BindAction(moodView.BackButton, OnBackButtonClicked)
                .BindAsyncAction(moodView.StartButton, OnStartClick);
      
           // await moodView.InitializeStartButton(()=>OnStartClick().Forget());
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        { 
            moodView.ResetMoods();
            await moodView.Show(true);
        }

        public override async UniTask ExecuteAsync()
        { }

        public override async UniTask ExitAsync()
        {
            await moodView.Hide(true);
        }
        
        private async UniTask OnStartClick()
        {
            // var settings = await breathingApi.GetBreathingSettingsForCurrentPartOfTheDay();
            //
            // StateMachine.SetStateAsync<BreathingState>(
            //         StateData.Create(("Settings", settings)), false)
            //     .Forget();
            
            // ask api to generate breathing settings
            var settings = await breathingApi.GetBreathingSettingsForCurrentPartOfTheDay();
            
            StateMachine.SetStateAsync<BreathingState>(
                    StateData.Create(("Settings", settings)), false)
                .Forget();
        }
      
        private void OnBackButtonClicked() => 
            StateMachine.SetStateAsync<MenuState>(waitForCurrentStateExit:false).Forget();
        
        private void OnMoodSelectionChanged(int moodIndex, bool isSelected)
        {
            if (isSelected)
            {
                selectedMoods.Add(moodIndex);
            }
            else
            {
                selectedMoods.Remove(moodIndex);
            }
        }
    }
}