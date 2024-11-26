using System;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui
{
    public class UiView : UiElement
    {
        public Button BackButton => backButton;
        [SerializeField] protected Button backButton;
     
        protected override void OnInit()
        { }

        public UiView BindAction(Button button, Action action)
        {
            button.onClick.AddListener(()=>action());
            return this;
        }
        public UiView BindAction(Button button, Func<UniTask> asyncAction)
        {
            button.onClick.AddListener(()=>asyncAction().Forget());
            return this;
        }
    }
}