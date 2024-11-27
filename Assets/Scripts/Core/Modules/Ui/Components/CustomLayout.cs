using System;
using UnityEngine;

namespace OneDay.Core.Modules.Ui.Components
{
    public abstract class CustomLayout : MonoBehaviour
    {
        [Serializable]
        public class WatchedField<T> where T: IEquatable<T>
        {
            public T Value;
            private T oldValue;

            public bool ProcessChange()
            {
                if (Value.Equals(oldValue)) return false;
                oldValue = Value;
                return true;
            }
        }
        
        private int childCount;
        
        protected abstract void Layout();

        protected void LateUpdate()
        {
            if (transform.childCount != childCount || OnDetectChangesInProperties())
            {
                Layout();
                childCount = transform.childCount;
               
            }
        }

        protected abstract bool OnDetectChangesInProperties();
    }
}