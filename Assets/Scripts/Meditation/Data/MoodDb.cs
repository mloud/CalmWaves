using System.Collections.Generic;
using UnityEngine;

namespace Meditation.Data
{
    [CreateAssetMenu(fileName = "MoodDb", menuName = "Breathing/MoodDb")]
    public class MoodDb : ScriptableObject, IMoodDb
    {
        [SerializeField] private int maxSelectedMoods;
        [SerializeField] private List<string> moods;

        public IEnumerable<string> Moods => moods;
        public int MaxMoodsSelected => maxSelectedMoods;
    }
}