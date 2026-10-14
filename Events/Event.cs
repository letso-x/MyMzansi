using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMzansi.Events
{
    public enum MunicipalEventType
    {
        Event,
        Announcement
    }

    public sealed class Event
    {
        private readonly List<string> tags;

        public string Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime? EndDateTime { get; private set; }
        public string Location { get; private set; }
        public MunicipalEventType Type { get; private set; }
        public int Importance { get; private set; }
        public IList<string> Tags { get { return tags.AsReadOnly(); } }

        public Event(string id, string title, string description, string category, DateTime date,
            DateTime? endDateTime, string location, MunicipalEventType type, int importance, IEnumerable<string> tags)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("An event ID is required.", "id");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("An event title is required.", "title");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("An event description is required.", "description");
            if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("An event category is required.", "category");
            if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("An event location is required.", "location");
            if (endDateTime.HasValue && endDateTime.Value < date) throw new ArgumentException("The end time cannot be earlier than the start time.", "endDateTime");
            if (importance < 1 || importance > 5) throw new ArgumentOutOfRangeException("importance", "Importance must be between 1 and 5.");

            Id = id.Trim();
            Title = title.Trim();
            Description = description.Trim();
            Category = NormalizeCategory(category);
            Date = date;
            EndDateTime = endDateTime;
            Location = location.Trim();
            Type = type;
            Importance = importance;
            this.tags = (tags ?? Enumerable.Empty<string>())
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static string NormalizeCategory(string category)
        {
            return string.Join(" ", (category ?? string.Empty).Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
