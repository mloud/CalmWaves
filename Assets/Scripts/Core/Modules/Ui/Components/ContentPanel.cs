using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneDay.Core.Modules.Ui.Components
{
    public class ContentPanel<T> : MonoBehaviour where T: Component
    {
        [SerializeField] private RectTransform container;
        [SerializeField] private T prefab;
        private List<T> items;

        public IEnumerable<T> Items => items;
        public T Get(int index) => container.GetChild(index).GetComponent<T>();
        public int Count => items.Count;

        public void Prepare(int count)
        {
            items?.Clear();
            items ??= new List<T>();   
            
            foreach (Transform child in container)
            {
                child.gameObject.SetActive(false);
            }

            int max = Math.Max(container.childCount, count);
           
            for (int i = 0; i < max; i++)
            {
                if (i >= count)
                {
                    container.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    if (i >= container.childCount)
                    {
                        items.Add(Instantiate(prefab, container));
                    }
                    else
                    {
                        var item = container.GetChild(i).GetComponent<T>();
                        Debug.Assert(item !=null);
                        items.Add(item);
                        item.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}