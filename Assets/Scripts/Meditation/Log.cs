using System.Collections.Generic;
using UnityEngine;

namespace Meditation
{
    public static class Log
    {
        public static string AllTags = "all";

        private static HashSet<string> activeTags = new();

        private static bool allTagsActive;
        
        public static void EnableTag(string tag)
        {
            if (tag == AllTags)
            {
                allTagsActive = true;
                return;
            }

            activeTags.Add(tag);
        }

        public static void DisableTag(string tag)
        {
            if (tag == AllTags)
            {
                allTagsActive = false;
                return;
            }

            activeTags.Remove(tag);
        }
        
        public static void LogInfo(string message, string tag)
        {
            if (IsTagActive(tag))
            {
                Debug.Log(message);
            }
        }
        
        public static void LogWarning(string message, string tag)
        {
            if (IsTagActive(tag))
            {
                Debug.LogWarning(message);
            }
        }
        
        public static void LogError(string message, string tag)
        {
            if (IsTagActive(tag))
            {
                Debug.LogError(message);
            }
        }

        private static bool IsTagActive(string tag) => allTagsActive || activeTags.Contains(tag);
    }
}