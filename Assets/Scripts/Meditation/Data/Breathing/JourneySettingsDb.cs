using System.Collections.Generic;
using UnityEngine;

namespace Meditation.Data.Breathing
{
    [System.Serializable]
    public class ChapterInfo
    {
        public string Name;
        public string Description;
        public int FromMission;
    }
    
    [CreateAssetMenu(fileName = "JourneyExerciseDb", menuName = "Breathing/JourneyExerciseDb")]
    public class JourneySettingsDb : ScriptableObject
    {
        public string Id => JourneyId;
        public string Name => JourneyName;

        [SerializeField] private string JourneyId;
        [SerializeField] private string JourneyName;
        [SerializeField] private List<JourneyBreathingSettings> journey;
        [SerializeField] private List<ChapterInfo> chapterInfos;

        public JourneyBreathingSettings GetBreathingSettings(int order) => journey[order];
        public IEnumerable<JourneyBreathingSettings> GetMissions() => journey;
        public IEnumerable<ChapterInfo> GetChapterInfos() => chapterInfos;
    }
}