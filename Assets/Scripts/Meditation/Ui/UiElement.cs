using UnityEngine;

namespace Meditation.Ui
{
    public class UiElement : ABaseElement
    {
        [SerializeField] private bool registerByName;
        
        protected override void OnInit()
        {
            if (registerByName)
            {
                LookUp.Get<UiElement>().Register(this);
            }
        }

        protected override void OnDeInit()
        {
            if (registerByName)
            {
                LookUp.Get<UiElement>().Register(this);
            }
        }
    }
}