using System.Collections.Generic;
using UnityEngine;

namespace OneDay.Core.Debugging
{
    public class DebugSections : MonoBehaviour
    {
        [System.Serializable]
        public class Section
        {
            public string Name;
            public bool Enabled;
        }
        [SerializeField] private List<Section> sections;

        public IEnumerable<Section> Sections => sections;
    }
}