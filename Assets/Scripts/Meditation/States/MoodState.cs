using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Apis;
using Meditation.Data;
using Meditation.Ui.Components;
using Meditation.Ui.Views;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Data;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using UnityEngine;

namespace Meditation.States
{
    public class MoodState : AState
    {
        private MoodView moodView;
        private AddressableAsset<GameObject> moodPrefab;
        private AddressableAsset<MoodDb> moodDbAsset;
        private IBreathGeneratorApi generatorApi;
        private HashSet<int> selectedMoods;
        
        public override async UniTask Initialize()
        {
            selectedMoods = new HashSet<int>();
            generatorApi = ServiceLocator.Get<IBreathGeneratorApi>();
            moodDbAsset = await ServiceLocator.Get<IDataManager>().GetMoodSettings();
            moodPrefab = await ServiceLocator.Get<IAssetManager>().GetAssetAsync<GameObject>("MoodButton");
            moodView = ServiceLocator.Get<IUiManager>().GetView<MoodView>();
            moodView.Initialize(moodDbAsset.GetReference(), moodPrefab.GetReference().GetComponent<Mood>(), OnMoodSelectionChanged);
            moodView
                .BindAction(moodView.BackButton, OnBackButtonClicked)
                .BindAction(moodView.GenerateButton.Button, OnStartClick);
      
            //await moodView.InitializeStartButton(()=>OnStartClick().Forget());
        }

        public override async UniTask EnterAsync(StateData stateData = null)
        { 
            moodView.Reset();
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
            await moodView.SwitchToGenerateMode();
            var moods = selectedMoods
                .Select(i => moodDbAsset.GetReference().Moods.ElementAt(i));

            var settings = await generatorApi.Generate(moods);
            
            // var settings = await breathingApi.GetBreathingSettingsForCurrentPartOfTheDay();
            //
            // StateMachine.SetStateAsync<BreathingState>(
            //         StateData.Create(("Settings", settings)), false)
            //     .Forget();
            
            // ask api to generate breathing settings
            //var settings = await breathingApi.GetBreathingSettingsForCurrentPartOfTheDay();
            
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