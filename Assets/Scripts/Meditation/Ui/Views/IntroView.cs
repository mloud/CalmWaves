using System.Collections.Generic;
using UnityEngine;

namespace Meditation.Ui.Views
{
    public class IntroView : UiView
    {
        public List<GameObject> Parts => parts;
        [SerializeField] private List<GameObject> parts;
    }
}