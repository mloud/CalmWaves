using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Data.Breathing;
using OneDay.Core;
using OneDay.Core.Modules.Assets;
using OneDay.Core.Modules.Data;
using UnityEngine;

namespace Meditation.Apis
{
    public interface IJourneyManager
    {
        UniTask<JourneyProgression> GetProgression(string journeyId);
        UniTask<IEnumerable<JourneySettingsDb>> GetJourneys();
        UniTask<JourneySettingsDb> GetJourney(string journeyId);

        UniTask FinishJourneyMission(string journeyId, int missionOrder);
    }

    public class JourneyManager : MonoBehaviour, IService, IJourneyManager
    {
        private AddressableAsset<JourneySettingsDb> journeyDbAsset;
        private IDataManager dataManager;

        public async UniTask Initialize()
        {
            dataManager = ServiceLocator.Get<IDataManager>();
            // definition
            journeyDbAsset = await dataManager.GetJourneySettings();
        }

        public UniTask PostInitialize() => UniTask.CompletedTask;
        
        public async UniTask<JourneySettingsDb> GetJourney(string journeyId)
        {
            return (await GetJourneys()).FirstOrDefault(x => x.Id == journeyId);
        }
        
        public async UniTask<JourneyProgression> GetProgression(string journeyId)
        {
            var journeyProgressions = await dataManager.GetAll<JourneyProgression>();
            var progression =  journeyProgressions.FirstOrDefault(x => x.JourneyId == journeyId);
            return progression;
        }

        public async UniTask<IEnumerable<JourneySettingsDb>> GetJourneys()
        {
            return new List<JourneySettingsDb> {journeyDbAsset.GetReference()};
        }

        public async UniTask FinishJourneyMission(string journeyId, int missionOrder)
        {
            var journeyDefinition = await GetJourney(journeyId);
            Debug.Assert(journeyDefinition != null, $"No such Journey definition {journeyId} exists");

            var progression = await GetProgression(journeyId);

            if (progression == null)
            {
                Debug.Assert(missionOrder == 0, 
                    $"There is no progression for journey {journeyId} yet, the only missionOrder=0, expected");

                await dataManager.Add(new JourneyProgression
                {
                    JourneyId = journeyId,
                    CurrentProgress = 1,
                    LastFinishedTime = DateTime.Now
                });
            }
            else
            {
                Debug.Assert((missionOrder <= progression.CurrentProgress) ||
                             (missionOrder == progression.CurrentProgress + 1));
        
                if (missionOrder == (progression.CurrentProgress + 1))
                {
                    progression.CurrentProgress = missionOrder + 1;
                    progression.LastFinishedTime = DateTime.Now;
                }
                await dataManager.Actualize(progression);
            }
        }
    }
}