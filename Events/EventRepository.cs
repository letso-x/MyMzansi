using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMzansi.Events
{
    public sealed class EventPriority : IComparable<EventPriority>
    {
        public Event Event { get; private set; }

        public EventPriority(Event item)
        {
            Event = item ?? throw new ArgumentNullException("item");
        }

        public int CompareTo(EventPriority other)
        {
            if (other == null) return 1;
            bool thisUpcoming = Event.Date.Date >= DateTime.Today;
            bool otherUpcoming = other.Event.Date.Date >= DateTime.Today;
            int comparison = otherUpcoming.CompareTo(thisUpcoming);
            if (comparison != 0) return comparison;
            comparison = Event.Date.Date.CompareTo(other.Event.Date.Date);
            if (comparison != 0) return comparison;
            comparison = other.Event.Importance.CompareTo(Event.Importance);
            if (comparison != 0) return comparison;
            comparison = Event.Date.TimeOfDay.CompareTo(other.Event.Date.TimeOfDay);
            return comparison != 0 ? comparison : string.Compare(Event.Id, other.Event.Id, StringComparison.OrdinalIgnoreCase);
        }
    }

    public sealed class EventRepository
    {
        private readonly Dictionary<string, Event> eventsById;
        private readonly SortedDictionary<DateTime, List<Event>> eventsByDate;
        private readonly HashSet<string> categories;
        private readonly HashSet<DateTime> eventDates;
        private readonly List<Event> allEvents;

        public EventRepository(IEnumerable<Event> events)
        {
            if (events == null) throw new ArgumentNullException("events");
            eventsById = new Dictionary<string, Event>(StringComparer.OrdinalIgnoreCase);
            eventsByDate = new SortedDictionary<DateTime, List<Event>>();
            categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            eventDates = new HashSet<DateTime>();
            allEvents = new List<Event>();

            foreach (Event item in events) AddEvent(item);
        }

        public IList<Event> AllEvents { get { return allEvents.AsReadOnly(); } }
        public IEnumerable<string> Categories { get { return categories.OrderBy(value => value, StringComparer.CurrentCultureIgnoreCase); } }
        public IEnumerable<DateTime> EventDates { get { return eventDates.OrderBy(value => value); } }
        public IEnumerable<DateTime> IndexedDates { get { return eventsByDate.Keys; } }

        public void AddEvent(Event item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (eventsById.ContainsKey(item.Id)) throw new ArgumentException("Event IDs must be unique: " + item.Id, "item");

            eventsById.Add(item.Id, item);
            allEvents.Add(item);
            DateTime date = item.Date.Date;
            List<Event> dayEvents;
            if (!eventsByDate.TryGetValue(date, out dayEvents))
            {
                dayEvents = new List<Event>();
                eventsByDate.Add(date, dayEvents);
            }
            dayEvents.Add(item);
            dayEvents.Sort(CompareByDateAndId);
            eventDates.Add(date);
            categories.Add(Event.NormalizeCategory(item.Category));
        }

        public bool TryGetById(string id, out Event item)
        {
            item = null;
            return !string.IsNullOrWhiteSpace(id) && eventsById.TryGetValue(id.Trim(), out item);
        }

        public IList<Event> GetEventsOn(DateTime date)
        {
            List<Event> dayEvents;
            return eventsByDate.TryGetValue(date.Date, out dayEvents) ? dayEvents.AsReadOnly() : new List<Event>().AsReadOnly();
        }

        public IList<Event> GetUpcomingByPriority()
        {
            // Priority rules: upcoming items first, then calendar date, importance, start time, and ID.
            // SortedSet provides minimum-priority dequeue behavior on .NET Framework 4.8.
            SortedSet<EventPriority> queue = new SortedSet<EventPriority>();
            foreach (Event item in allEvents) queue.Add(new EventPriority(item));
            List<Event> ordered = new List<Event>(queue.Count);
            while (queue.Count > 0)
            {
                EventPriority next = queue.Min;
                queue.Remove(next);
                ordered.Add(next.Event);
            }
            return ordered.AsReadOnly();
        }

        public static EventRepository CreateDemonstrationRepository()
        {
            DateTime today = DateTime.Today;
            return new EventRepository(new[]
            {
                Create("EVT-001", "Community clean-up campaign", "Join neighbours and municipal teams to clean public spaces and discuss ongoing waste reduction.", "Community", today.AddDays(2).AddHours(9), "Dlamini Community Hall, Soweto", MunicipalEventType.Event, 3, "clean-up", "environment", "community"),
                Create("EVT-002", "Water-supply maintenance notice", "Planned maintenance may temporarily affect water supply in the area. Please store water for essential use.", "Water Services", today.AddDays(3).AddHours(8), "Ward 14 service area, Cape Town", MunicipalEventType.Announcement, 5, "water", "maintenance", "supply"),
                Create("EVT-003", "Local market day", "Browse local produce, crafts and small-business stalls. Family activities will be available during the morning.", "Markets", today.AddDays(5).AddHours(8), "Civic Centre Square, Durban", MunicipalEventType.Event, 2, "market", "small business", "family"),
                Create("EVT-004", "Public meeting: community safety", "Residents are invited to discuss neighbourhood safety priorities with municipal representatives and local partners.", "Public Meetings", today.AddDays(5).AddHours(17), "Tshwane Community Library, Pretoria", MunicipalEventType.Event, 4, "safety", "meeting", "residents"),
                Create("EVT-005", "Electricity maintenance announcement", "Scheduled electrical network work is planned. Check local notices for the affected streets and prepare for a possible interruption.", "Electricity", today.AddDays(7).AddHours(7), "Central maintenance area, Johannesburg", MunicipalEventType.Announcement, 5, "electricity", "maintenance", "power"),
                Create("EVT-006", "Public health awareness day", "Meet local health partners for wellness information, prevention resources and general health awareness activities.", "Public Health", today.AddDays(9).AddHours(10), "Community Sports Ground, Gqeberha", MunicipalEventType.Event, 3, "health", "wellness", "awareness"),
                Create("EVT-007", "Waste collection schedule change", "A temporary route adjustment may affect collection times. Please place household waste out on the revised collection day.", "Waste Services", today.AddDays(11).AddHours(6), "Ward 3, Mbombela", MunicipalEventType.Announcement, 4, "waste", "collection", "schedule"),
                Create("EVT-008", "Municipal budget consultation", "Residents can share priorities and feedback during the public consultation on proposed municipal budget plans.", "Public Meetings", today.AddDays(14).AddHours(18), "Municipal chambers, Polokwane", MunicipalEventType.Event, 5, "budget", "consultation", "public meeting"),
                Create("EVT-009", "Parks and recreation morning", "Take part in outdoor recreation activities and learn about planned improvements to local parks.", "Parks and Recreation", today.AddDays(14).AddHours(9), "Riverside Park, East London", MunicipalEventType.Event, 2, "parks", "recreation", "family"),
                Create("EVT-010", "Water conservation information session", "A practical session on household water conservation and reporting leaks to the municipality.", "Water Services", today.AddDays(19).AddHours(15), "Ward office, Kimberley", MunicipalEventType.Event, 3, "water", "conservation", "leaks"),
                Create("EVT-011", "Community clean-up: coastal route", "Help remove litter along a public coastal route and learn how to report illegal dumping.", "Community", today.AddDays(23).AddHours(8), "Beach access point, uMhlathuze", MunicipalEventType.Event, 3, "clean-up", "environment", "illegal dumping"),
                Create("EVT-012", "Municipal service interruption update", "An informational notice about a scheduled service interruption and where residents can find municipal assistance.", "Municipal Notices", today.AddDays(28).AddHours(12), "Online and municipal customer centre, Rustenburg", MunicipalEventType.Announcement, 3, "service", "notice", "assistance")
            });
        }

        private static Event Create(string id, string title, string description, string category, DateTime date,
            string location, MunicipalEventType type, int importance, params string[] tags)
        {
            return new Event(id, title, description, category, date, null, location, type, importance, tags);
        }

        private static int CompareByDateAndId(Event first, Event second)
        {
            int comparison = first.Date.CompareTo(second.Date);
            return comparison != 0 ? comparison : string.Compare(first.Id, second.Id, StringComparison.OrdinalIgnoreCase);
        }
    }
}
