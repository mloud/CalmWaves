using System;

namespace OneDay.Core.Debugging
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class LogSectionAttribute : Attribute
    {
        public string SectionName { get; }

        public LogSectionAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
    }
}