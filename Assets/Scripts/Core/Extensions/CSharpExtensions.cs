using System;
using System.Collections.Generic;
using System.Linq;

namespace OneDay.Core.Extensions
{
    public static class CSharpExtensions
    {
        public static List<string> GetListeners<T>(this Action<T> action)
        {
            List<string> listeners = null;
            if (action != null)
            {
                listeners = action.GetInvocationList().Select(x => x.Method.Name).ToList();
            }

            return listeners ?? new List<string>();
        }

        public static void RemoveWhen<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}