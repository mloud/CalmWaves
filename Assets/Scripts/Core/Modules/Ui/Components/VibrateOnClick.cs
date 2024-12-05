using OneDay.Core.Modules.Vibrations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDay.Core.Modules.Ui.Components
{
    public class VibrateOnClick : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            ServiceLocator.Get<IVibrationManager>().VibratePeek();
        }
    }
}