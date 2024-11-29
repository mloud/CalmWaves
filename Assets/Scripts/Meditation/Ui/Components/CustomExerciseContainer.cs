using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meditation.Data;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;


namespace Meditation.Ui.Components
{
    public class CustomExerciseContainer : MonoBehaviour
    {
        public Button CreateNewButton => createNewButton;
        public Action<CustomBreathingSettings> BreathingSettingsSelected { get; set; }
        public Action<CustomBreathingSettings> BreathingSettingsDeleted { get; set; }
        
        [SerializeField] private Button createNewButton;
        [SerializeField] private CustomBreathingButton buttonPrefab;
        [SerializeField] private List<CustomBreathingButton> buttons;
        [SerializeField] private Transform container;
        [SerializeField] private ScrollRect scrollView;
        
        public async UniTask Initialize(IEnumerable<CustomBreathingSettings> breathingSettings)
        {
            const int builtInButtons = 4;
            var sortedBreathingSettings = breathingSettings.OrderByDescending(x => x.CreateTime);
            int index = 0;
            foreach (var settings in sortedBreathingSettings)
            {
                if (index >= buttons.Count)
                {
                    buttons.Add(Instantiate(buttonPrefab, container));
                }
                buttons[index].Set(
                    settings, 
                    s => BreathingSettingsSelected(s),
                    s=>BreathingSettingsDeleted(s));
                index++;
            }

            for (; index < builtInButtons; index++)
            {
                buttons[index].Set();
            }

            ScrollTobBeginning();
        }
        
        public void ScrollTobBeginning() => scrollView.horizontalNormalizedPosition = 0;

        public async UniTask Add(IBreathingSettings breathingSetting)
        {
            
        }
        
        public async UniTask Remove(string id)
        {
            
        }
        
    }
}