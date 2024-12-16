using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneDay.Core.Debugging
{
    public static class D
    {
        private static readonly Dictionary<string, bool> logSections = new();
        private static readonly Dictionary<object, string> objectSectionCache = new();

        public static void Initialize(IEnumerable<DebugSections.Section> sections)
        {
            foreach (var section in sections)
            {
                if (section.Enabled)
                {
                    EnableLogging(section.Name);
                }
                else
                {
                    DisableLogging(section.Name);
                }
            }
        }        
        public static void EnableLogging(params string[] sections)
        {
            foreach (var section in sections)
            {
                if (!logSections.TryAdd(section, true))
                {
                    logSections[section] = true;
                }  
            }
        }

        public static void DisableLogging(params string[] sections)
        {
            foreach (var section in sections)
            {
                logSections.Remove(section);
            }
        }
        
        public static void LogInfo(string message, string section = null)
        {
            if (IsSectionEnabled(section))
            {
                Debug.Log(CreateLogMessage(section, message));
            }
        }
        
        public static void LogInfo(string message, object context)
        {
            var section = TryGetLogSection(context);
            if (IsSectionEnabled(section))
            {
                Debug.Log(CreateLogMessage(section, message));
            }
        }
        
        public static void LogWarning(string message, string section = null)
        {
            if (IsSectionEnabled(section))
            {
                Debug.LogWarning(CreateLogMessage(section, message));
            }
        }
        
        public static void LogWarning(string message, object context)
        {
            var section = TryGetLogSection(context);
            if (IsSectionEnabled(section))
            {
                Debug.LogWarning(CreateLogMessage(section, message));
            }
        }
        
        public static void LogError(string message, string section = null) => 
            Debug.LogError(CreateLogMessage(section, message));

        public static void LogError(string message, object context = null)
        {
            var section = TryGetLogSection(context);
            if (IsSectionEnabled(section))
            {
                Debug.LogError(CreateLogMessage(section, message));
            }
        }

        private static string CreateLogMessage(string section, string message) =>
            string.IsNullOrEmpty(section) 
                ? message 
                : $"[{section.ToUpper()}] {message}";

        private static bool IsSectionEnabled(string section) => 
            section == null || logSections.GetValueOrDefault(section, false);

        private static string TryGetLogSection(object instance)
        {
            if (instance == null) return null;
            
            if (objectSectionCache.TryGetValue(instance, out var section))
                return section;
            
            var type = instance.GetType();
            var attribute = Attribute.GetCustomAttribute(type, typeof(LogSectionAttribute)) as LogSectionAttribute;

            objectSectionCache.TryAdd(instance, attribute?.SectionName);
            return attribute?.SectionName;
        }
    }
}