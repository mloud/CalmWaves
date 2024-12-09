using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OneDay.Core.Modules.Ui.Components
{
    public class CToggleGroup : MonoBehaviour
    {
        public int MinSelectedToggles = 0;
        public int MaxSelectedToggles = -1;
        public bool IsUsingSwitchBehaviour => useSwitchBehaviour;
        public IEnumerable<CToggle> Toggles => toggles;
        
        [SerializeField] private List<CToggle> toggles;
        [SerializeField] private bool useSwitchBehaviour; 
        public void RegisterToggle(CToggle toggle)
        {
            if (toggles.Contains(toggle))
                return;
            
            toggles.Add(toggle);
        }

        public void UnregisterToggle(CToggle toggle)
        {
            toggles.Remove(toggle);
        }

        public bool CanSetToggle(bool state)
        {
            if (state)
            {
                return MaxSelectedToggles == -1 || toggles.Count(x => x.IsOn) < MaxSelectedToggles;
            }
            return toggles.Count(x => x.IsOn) > MinSelectedToggles;
        }
    }
}