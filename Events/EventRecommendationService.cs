using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMzansi.Events
{
    public sealed class EventRecommendation
    {
        public Event Event { get; private set; }
        public int Score { get; private set; }
        public string Reason { get; private set; }

        public EventRecommendation(Event item, int score, string reason)
        {
            Event = item ?? throw new ArgumentNullException("item");
            Score = score;
            Reason = reason ?? string.Empty;
        }
    }

    public sealed class EventRecommendationService
    {
        private readonly EventRepository repository;
        private readonly EventSearchService searchService;

        public EventRecommendationService(EventRepository repository, EventSearchService searchService)
        {
            this.repository = repository ?? throw new ArgumentNullException("repository");
            this.searchService = searchService ?? throw new ArgumentNullException("searchService");
        }

        public IList<EventRecommendation> Recommend(string excludedEventId, int maximumResults)
        {
            if (maximumResults < 1) return new List<EventRecommendation>().AsReadOnly();

            SearchRecord[] searches = searchService.RecentSearches.ToArray();
            List<EventRecommendation> recommendations = new List<EventRecommendation>();
            foreach (Event item in repository.AllEvents)
            {
                if (item.Date.Date < DateTime.Today) continue;
                if (!string.IsNullOrWhiteSpace(excludedEventId) && string.Equals(item.Id, excludedEventId, StringComparison.OrdinalIgnoreCase)) continue;

                int score = 0;
                List<string> reasons = new List<string>();
                HashSet<string> reasonSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (SearchRecord search in searches)
                {
                    if (!string.IsNullOrWhiteSpace(search.Category)
                        && string.Equals(Event.NormalizeCategory(item.Category), search.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        score += 5;
                        AddReason(reasons, reasonSet, "Matches your interest in " + item.Category.ToLowerInvariant() + ".");
                    }

                    foreach (string term in GetKeywords(search.SearchTerms))
                    {
                        if (MatchesKeyword(item, term))
                        {
                            score += 2;
                            AddReason(reasons, reasonSet, "Matches a keyword from your searches: " + term + ".");
                        }
                    }
                }

                if (score > 0)
                {
                    score += 2;
                    score += item.Importance;
                    AddReason(reasons, reasonSet, "Upcoming item; importance level " + item.Importance + " of 5.");
                    recommendations.Add(new EventRecommendation(item, score, string.Join(" ", reasons)));
                }
            }

            if (searches.Length == 0 || recommendations.Count == 0)
            {
                return repository.GetUpcomingByPriority()
                    .Where(item => item.Date.Date >= DateTime.Today
                        && (string.IsNullOrWhiteSpace(excludedEventId) || !string.Equals(item.Id, excludedEventId, StringComparison.OrdinalIgnoreCase)))
                    .Take(maximumResults)
                    .Select(item => new EventRecommendation(item, item.Importance + 2, "Suggested as one of the next upcoming municipal items."))
                    .ToList().AsReadOnly();
            }

            return recommendations
                .GroupBy(item => item.Event.Id, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Event.Date)
                .ThenBy(item => item.Event.Id, StringComparer.OrdinalIgnoreCase)
                .Take(maximumResults)
                .ToList().AsReadOnly();
        }

        private static IEnumerable<string> GetKeywords(string searchTerms)
        {
            return (searchTerms ?? string.Empty).Split(new[] { ' ', '\t', '\r', '\n', ',', '.', ';', ':', '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(term => term.Trim())
                .Where(term => term.Length >= 3)
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static void AddReason(ICollection<string> reasons, ISet<string> uniqueReasons, string reason)
        {
            if (uniqueReasons.Add(reason)) reasons.Add(reason);
        }

        private static bool MatchesKeyword(Event item, string keyword)
        {
            return item.Title.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0
                || item.Description.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0
                || item.Category.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0
                || item.Location.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0
                || item.Tags.Any(tag => tag.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }
    }
}
