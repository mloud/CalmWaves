using System;
using System.Collections.Generic;
using System.Linq;

namespace Meditation.Apis.Data
{
    public class Calendar<TEvent>
    {
        private Dictionary<int, List<TEvent>> Events { get; } = new();

        public void AddEvents(params (TEvent evt, DateTime dateTime)[] events)
        {
            foreach (var (evt, dateTime) in events)
            {
                AddEvent(evt, dateTime);
            }
        }
        
        public void AddEvent(TEvent evt, DateTime dateTime)
        {
            var key = GetKeyYYYYMMDDKeyForDate(dateTime);
            if (!Events.TryGetValue(key, out var eventList))
            {
                eventList = new List<TEvent>();
                Events[key] = eventList;
            }
            eventList.Add(evt);
        }

        public IReadOnlyList<TEvent> GetEvents(DateTime dateTime)
        {
            var key = GetKeyYYYYMMDDKeyForDate(dateTime);
            return Events.TryGetValue(key, out var eventList) ? eventList : Array.Empty<TEvent>();
        }

        public IReadOnlyList<TEvent> GetAllEvents() =>
            Events.SelectMany(x => x.Value).ToList();
        
        private static int GetKeyYYYYMMDDKeyForDate(DateTime dateTime) => dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
    }
}