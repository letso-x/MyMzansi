# MyMzansi Municipal Services

MyMzansi is a Windows Forms municipal-services demonstration application for South Africa. It combines the Part 1 issue-reporting workflow with Part 2 browsing and searching local-event and municipal-announcement demonstration data.

## Features

### Part 1: Report Issues

- Main-menu navigation to the existing issue dashboard and report form.
- Issue subject, location, category, description, and optional image/document attachment.
- Required-field validation, completion progress, and issue detail viewing.
- Session-only issue storage; reports are not saved after the application closes.

### Part 2: Local Events and Announcements

- Case-insensitive search across titles, descriptions, categories, locations, and tags/keywords.
- Combined text search, unique category, start/end date, and event/announcement type filters.
- Chronological results, clear filters, event details, and a helpful empty-results message.
- Recent searches can be repeated by double-clicking an entry in the Recent searches list.
- Viewing an item records it in the recently viewed list, limited to the latest 10 unique event IDs.
- Recommendations display a reason, refresh after searches, and exclude the event currently being viewed.
- Service Request Status opens an informational placeholder. Service request tracking is not implemented.

## Prerequisites

- Windows, because the application uses Windows Forms.
- Visual Studio with .NET desktop development support (Visual Studio Community 2026 is suitable).
- The **.NET Framework 4.8 Developer Pack / Targeting Pack**.
- No third-party NuGet packages, external database, or API are required.

## Open, build, and run

1. Open `MyMzansi.slnx` in Visual Studio. If your installation does not recognize `.slnx`, open `MyMzansi.csproj` directly.
2. Confirm the project targets **.NET Framework 4.8** and the targeting pack is installed.
3. Choose **Build > Build Solution**.
4. Start with **Debug > Start Without Debugging** or Visual Studio's Start button.
5. The application starts at the main menu; `Program.Main` launches `Home`.

## Using the application

### Report an issue

Choose **Report Issues**, enter the subject, location, category, and description, and optionally attach evidence. Submit the report, then use the issue dashboard to review its summary or details. Issue data is held in memory for the current process only.

### Browse events and announcements

Choose **Local Events and Announcements**. Type a phrase or keyword and/or choose a category, start date, end date, and item type. Filters refresh the results as they change; **Search** records the current filter combination in search history. Press Enter in the text field to search, or use **Clear filters** to reset the filters. Date selectors contain the distinct dates represented in the sample listings. A start date after the end date is rejected with a clear message.

Select a result and choose **View selected details**, or double-click it, to see the description, category, type, date/time, location, importance, and tags. Double-click a recommendation to open its details. Double-click a recent-search entry to repeat it. Double-click a recently viewed event to reopen it. The recent-view list retains up to 10 unique IDs; viewing an item again moves it to the newest position.

### Recommendations

Recommendations are deterministic and use recent search history. Each matching previous category search adds **5** points. Each distinct search term of at least three characters found in an event's title, description, category, location, or tags adds **2** points for that search. An interest-matched upcoming item then receives **2** upcoming points plus its **1–5** importance value. Past items are excluded. Results sort by score descending, then date/time ascending and ID. If there is no history or no interest match, the app suggests the next upcoming items, ordered by calendar date, importance for same-date ties, start time, and ID. Each recommendation displays its matching interest/keyword and the upcoming/importance reason. No machine learning or external recommendation service is used.

## Data structures and algorithms

| Structure | Purpose | Use in the application |
| --- | --- | --- |
| `Dictionary<string, Event>` | Hash-table lookup by unique event ID | `EventRepository` validates duplicate IDs and resolves details/recently viewed IDs efficiently. C# `Dictionary<TKey,TValue>` is hash-table-based. |
| `SortedDictionary<DateTime, List<Event>>` | Chronological index grouped by calendar date | Keeps multiple events on a date together and exposes the date index chronologically. |
| `HashSet<string>` (categories) | Unique, normalized categories | Deduplicates categories case-insensitively and supplies the category filter. |
| `HashSet<DateTime>` (event dates) | Unique calendar dates | Supplies date selectors without duplicate dates. |
| `Stack<SearchRecord>` | Most-recent-first search history | Records explicit searches, suppresses consecutive duplicate filter combinations, and supports repeat searches. |
| `Queue<string>` and `HashSet<string>` | Bounded recent-view order and duplicate detection | Moves repeat views to the newest position and dequeues oldest IDs beyond the 10-item limit. |
| `SortedSet<EventPriority>` | .NET Framework 4.8-compatible priority-queue equivalent | Drains in deterministic order: upcoming before past, calendar date, higher importance for same-date ties, start time, then ID. Items are removed by minimum priority; display does not depend on set enumeration order. |
| `List<Event>` | Source collection for iteration and listing | Holds event objects used by search and recommendations. |

The event listings are in-memory **demonstration data**. They are illustrative examples, not verified municipal notices or real-world schedules. There is no live municipal data feed or external persistence.

## Troubleshooting

- **Framework/targeting-pack error:** install the .NET Framework 4.8 Developer Pack, then reload and rebuild.
- **Solution file not recognized:** open `MyMzansi.csproj` directly in Visual Studio.
- **Build errors after adding source files:** verify the explicit `<Compile Include="..." />` entries in `MyMzansi.csproj`, then rebuild.
- **Windows Forms cannot run:** the application requires Windows and a graphical desktop session; a successful build does not exercise the interactive UI.
- **Events differ between runs:** dates are generated relative to the current date; listings are demonstration data only.

## Limitations

- Issues and events are in-memory; there is no database or file persistence.
- Events are local sample data and are not refreshed from a municipality or online service.
- Service Request Status is an informational placeholder; tracking is not implemented.
- The application is Windows-only because it targets Windows Forms on .NET Framework 4.8.
- Automated unit tests are not included in the existing repository.
