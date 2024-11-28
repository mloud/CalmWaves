using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace Meditation.Ui.Components
{
    public class CustomExerciseContainer : MonoBehaviour
    {
        public Button CreateNewButton => createNewButton;
        
        [SerializeField] private Button createNewButton;

        public async UniTask Initialize(IEnumerable<IBreathingSettings> breathingSettings)
        {
            
        }

        public async UniTask Add(IBreathingSettings breathingSetting)
        {
            
        }
    }
}