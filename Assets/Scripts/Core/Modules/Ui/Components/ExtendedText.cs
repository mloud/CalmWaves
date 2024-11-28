using UnityEngine;

namespace OneDay.Core.Modules.Ui.Components
{
    public abstract class AExtendedText : MonoBehaviour
    {
        public abstract string text { get; set; }
        public abstract void Set(string text);
    }
}