using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMzansi.Events
{
    public sealed class SearchRecord
    {
        public string SearchTerms { get; private set; }
        public string Category { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public MunicipalEventType? EventType { get; private set; }
        public DateTime SearchedAt { get; private set; }

        public SearchRecord(string searchTerms, string category, DateTime? startDate, DateTime? endDate,
            MunicipalEventType? eventType, DateTime searchedAt)
        {
            SearchTerms = (searchTerms ?? string.Empty).Trim();
            Category = string.IsNullOrWhiteSpace(category) ? null : Event.NormalizeCategory(category);
            StartDate = startDate.HasValue ? startDate.Value.Date : (DateTime?)null;
            EndDate = endDate.HasValue ? endDate.Value.Date : (DateTime?)null;
            EventType = eventType;
            SearchedAt = searchedAt;
        }

        internal bool HasSameFilters(SearchRecord other)
        {
            return string.Equals(SearchTerms, other.SearchTerms, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Category, other.Category, StringComparison.OrdinalIgnoreCase)
                && StartDate == other.StartDate
                && EndDate == other.EndDate
                && EventType == other.EventType;
        }
    }

    public sealed class EventSearchService
    {
        private readonly EventRepository repository;
        private readonly Stack<SearchRecord> searchHistory = new Stack<SearchRecord>();

        public EventSearchService(EventRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
        }

        public IList<SearchRecord> RecentSearches { get { return searchHistory.ToArray(); } }

        public IList<Event> Search(string searchTerms, string category, DateTime? startDate, DateTime? endDate,
            MunicipalEventType? eventType)
        {
            return Search(searchTerms, category, startDate, endDate, eventType, true);
        }

        public IList<Event> Preview(string searchTerms, string category, DateTime? startDate, DateTime? endDate,
            MunicipalEventType? eventType)
        {
            return Search(searchTerms, category, startDate, endDate, eventType, false);
        }

        private IList<Event> Search(string searchTerms, string category, DateTime? startDate, DateTime? endDate,
            MunicipalEventType? eventType, bool recordSearch)
        {
            if (startDate.HasValue && endDate.HasValue && startDate.Value.Date > endDate.Value.Date)
                throw new ArgumentException("The start date must be on or before the end date.");

            SearchRecord record = new SearchRecord(searchTerms, category, startDate, endDate, eventType, DateTime.Now);
            if (recordSearch && (searchHistory.Count == 0 || !searchHistory.Peek().HasSameFilters(record))) searchHistory.Push(record);

            IEnumerable<Event> results = repository.AllEvents;
            string query = record.SearchTerms;
            if (!string.IsNullOrWhiteSpace(query))
            {
                results = results.Where(item => Contains(item.Title, query)
                    || Contains(item.Description, query)
                    || Contains(item.Category, query)
                    || Contains(item.Location, query)
                    || item.Tags.Any(tag => Contains(tag, query)));
            }
            if (!string.IsNullOrWhiteSpace(record.Category))
                results = results.Where(item => string.Equals(Event.NormalizeCategory(item.Category), record.Category, StringComparison.OrdinalIgnoreCase));
            if (record.StartDate.HasValue)
                results = results.Where(item => item.Date.Date >= record.StartDate.Value);
            if (record.EndDate.HasValue)
                results = results.Where(item => item.Date.Date <= record.EndDate.Value);
            if (record.EventType.HasValue)
                results = results.Where(item => item.Type == record.EventType.Value);

            return results.OrderBy(item => item.Date).ThenBy(item => item.Id, StringComparer.OrdinalIgnoreCase).ToList().AsReadOnly();
        }

        public IList<Event> Repeat(SearchRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");
            return Search(record.SearchTerms, record.Category, record.StartDate, record.EndDate, record.EventType);
        }

        private static bool Contains(string value, string query)
        {
            return value != null && value.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }

    public sealed class EventActivityTracker
    {
        private readonly Queue<string> recentEventIds = new Queue<string>();
        private readonly HashSet<string> recentEventIdSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public int MaximumRecentItems { get; private set; }

        public EventActivityTracker(int maximumRecentItems)
        {
            if (maximumRecentItems < 1) throw new ArgumentOutOfRangeException("maximumRecentItems");
            MaximumRecentItems = maximumRecentItems;
        }

        public IList<string> RecentEventIds { get { return recentEventIds.ToArray(); } }

        public void RecordViewed(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId)) throw new ArgumentException("An event ID is required.", "eventId");
            string id = eventId.Trim();
            if (recentEventIdSet.Remove(id))
            {
                Queue<string> refreshed = new Queue<string>(recentEventIds.Where(value => !string.Equals(value, id, StringComparison.OrdinalIgnoreCase)));
                recentEventIds.Clear();
                foreach (string value in refreshed) recentEventIds.Enqueue(value);
            }
            recentEventIds.Enqueue(id);
            recentEventIdSet.Add(id);
            while (recentEventIds.Count > MaximumRecentItems)
            {
                string oldest = recentEventIds.Dequeue();
                recentEventIdSet.Remove(oldest);
            }
        }
    }
}
