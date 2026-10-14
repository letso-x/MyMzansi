using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MyMzansi.Events;

namespace MyMzansi
{
    public sealed class LocalEventsForm : Form
    {
        private sealed class DateChoice
        {
            public DateTime? Date { get; private set; }
            public string Label { get; private set; }
            public DateChoice(DateTime? date, string label) { Date = date; Label = label; }
            public override string ToString() { return Label; }
        }

        private readonly EventRepository repository;
        private readonly EventSearchService searchService;
        private readonly EventRecommendationService recommendationService;
        private readonly EventActivityTracker activityTracker;
        private readonly TextBox searchBox = new TextBox();
        private readonly ComboBox categoryFilter = new ComboBox();
        private readonly ComboBox startDateFilter = new ComboBox();
        private readonly ComboBox endDateFilter = new ComboBox();
        private readonly ComboBox typeFilter = new ComboBox();
        private readonly DataGridView resultsGrid = new DataGridView();
        private readonly ListBox recommendationsList = new ListBox();
        private readonly ListBox searchesList = new ListBox();
        private readonly ListBox recentlyViewedList = new ListBox();
        private readonly Label resultMessage = new Label();
        private string currentEventId;

        public LocalEventsForm()
        {
            repository = EventRepository.CreateDemonstrationRepository();
            searchService = new EventSearchService(repository);
            recommendationService = new EventRecommendationService(repository, searchService);
            activityTracker = new EventActivityTracker(10);
            InitializeInterface();
            PopulateFilters();
            RefreshSearch(false);
        }

        private void InitializeInterface()
        {
            Text = "Local Events and Announcements - MyMzansi";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(960, 650);
            Size = new Size(1220, 800);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(18),
                BackColor = BackColor
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 83));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(layout);

            Panel header = new Panel { Dock = DockStyle.Fill };
            Label title = new Label
            {
                Text = "Local Events and Announcements",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 53, 91),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            Label description = new Label
            {
                Text = "Discover upcoming community activities and municipal notices. Sample listings are demonstration data.",
                ForeColor = Color.FromArgb(75, 85, 99),
                AutoSize = true,
                Location = new Point(2, 42)
            };
            header.Controls.Add(title);
            header.Controls.Add(description);
            layout.Controls.Add(header, 0, 0);

            Panel filterPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(10) };
            TableLayoutPanel filterGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 2 };
            filterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));
            filterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
            filterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
            filterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
            filterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12));
            filterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            filterGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            filterGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            AddFilter(filterGrid, "Search title, details or keywords", searchBox, 0);
            AddFilter(filterGrid, "Category", categoryFilter, 1);
            AddFilter(filterGrid, "From date", startDateFilter, 2);
            AddFilter(filterGrid, "To date", endDateFilter, 3);
            AddFilter(filterGrid, "Item type", typeFilter, 4);
            Button searchButton = CreateButton("Search", Color.FromArgb(18, 53, 91));
            searchButton.Dock = DockStyle.Fill;
            searchButton.Click += delegate { RefreshSearch(true); };
            filterGrid.Controls.Add(searchButton, 5, 1);
            searchBox.KeyDown += delegate(object sender, KeyEventArgs args)
            {
                if (args.KeyCode == Keys.Enter) { args.SuppressKeyPress = true; RefreshSearch(true); }
            };
            searchBox.TextChanged += FilterChanged;
            categoryFilter.SelectedIndexChanged += FilterChanged;
            startDateFilter.SelectedIndexChanged += FilterChanged;
            endDateFilter.SelectedIndexChanged += FilterChanged;
            typeFilter.SelectedIndexChanged += FilterChanged;

            Button clearButton = CreateButton("Clear filters", Color.FromArgb(107, 114, 128));
            clearButton.Dock = DockStyle.Fill;
            clearButton.Click += ClearFilters;
            TableLayoutPanel filterLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            filterLayout.Controls.Add(filterGrid, 0, 0);
            filterLayout.Controls.Add(clearButton, 1, 0);
            filterPanel.Controls.Add(filterLayout);
            layout.Controls.Add(filterPanel, 0, 1);

            SplitContainer content = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                Size = new Size(1160, 500),
                SplitterDistance = 700,
                BackColor = BackColor,
                FixedPanel = FixedPanel.Panel2,
                Panel1MinSize = 400,
                Panel2MinSize = 280
            };
            layout.Controls.Add(content, 0, 2);
            BuildResultsPanel(content.Panel1);
            BuildActivityPanel(content.Panel2);
        }

        private void AddFilter(TableLayoutPanel panel, string label, Control control, int column)
        {
            Label filterLabel = new Label { Text = label, Dock = DockStyle.Fill, ForeColor = Color.FromArgb(55, 65, 81), TextAlign = ContentAlignment.BottomLeft };
            control.Dock = DockStyle.Fill;
            if (control is ComboBox)
            {
                ((ComboBox)control).DropDownStyle = ComboBoxStyle.DropDownList;
                ((ComboBox)control).IntegralHeight = false;
            }
            panel.Controls.Add(filterLabel, column, 0);
            panel.Controls.Add(control, column, 1);
        }

        private void BuildResultsPanel(Control parent)
        {
            TableLayoutPanel panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(0, 8, 10, 0) };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            Label heading = CreateHeading("Search results");
            ConfigureGrid();
            Panel buttonPanel = new Panel { Dock = DockStyle.Fill };
            Button viewButton = CreateButton("View selected details", Color.FromArgb(18, 53, 91));
            viewButton.Width = 170;
            viewButton.Dock = DockStyle.Left;
            viewButton.Click += delegate { ViewSelectedResult(); };
            Button backButton = CreateButton("Back to main menu", Color.FromArgb(107, 114, 128));
            backButton.Width = 160;
            backButton.Dock = DockStyle.Right;
            backButton.Click += delegate { Close(); };
            resultMessage.Text = "";
            resultMessage.AutoSize = true;
            resultMessage.ForeColor = Color.FromArgb(75, 85, 99);
            resultMessage.TextAlign = ContentAlignment.MiddleLeft;
            resultMessage.Dock = DockStyle.Fill;
            buttonPanel.Controls.Add(resultMessage);
            buttonPanel.Controls.Add(backButton);
            buttonPanel.Controls.Add(viewButton);
            panel.Controls.Add(heading, 0, 0);
            panel.Controls.Add(resultsGrid, 0, 1);
            panel.Controls.Add(buttonPanel, 0, 2);
            parent.Controls.Add(panel);
        }

        private void ConfigureGrid()
        {
            resultsGrid.Dock = DockStyle.Fill;
            resultsGrid.ReadOnly = true;
            resultsGrid.AllowUserToAddRows = false;
            resultsGrid.AllowUserToDeleteRows = false;
            resultsGrid.AllowUserToResizeRows = false;
            resultsGrid.MultiSelect = false;
            resultsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            resultsGrid.AutoGenerateColumns = false;
            resultsGrid.RowHeadersVisible = false;
            resultsGrid.BackgroundColor = Color.White;
            resultsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Title", FillWeight = 34, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            resultsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", FillWeight = 20, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            resultsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date and time", FillWeight = 22, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            resultsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Location", HeaderText = "Location", FillWeight = 24, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            resultsGrid.CellDoubleClick += delegate { ViewSelectedResult(); };
        }

        private void BuildActivityPanel(Control parent)
        {
            TableLayoutPanel panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, Padding = new Padding(0, 8, 0, 0) };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 44));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 28));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 28));
            ConfigureList(recommendationsList);
            ConfigureList(searchesList);
            ConfigureList(recentlyViewedList);
            panel.Controls.Add(CreateHeading("Recommended for you"), 0, 0);
            panel.Controls.Add(recommendationsList, 0, 1);
            panel.Controls.Add(CreateHeading("Recent searches"), 0, 2);
            panel.Controls.Add(searchesList, 0, 3);
            panel.Controls.Add(CreateHeading("Recently viewed events"), 0, 4);
            panel.Controls.Add(recentlyViewedList, 0, 5);
            recommendationsList.DoubleClick += delegate { ViewSelectedRecommendation(); };
            searchesList.DoubleClick += delegate { RepeatSelectedSearch(); };
            recentlyViewedList.DoubleClick += delegate { ViewSelectedRecent(); };
            parent.Controls.Add(panel);
        }

        private void ConfigureList(ListBox list)
        {
            list.Dock = DockStyle.Fill;
            list.IntegralHeight = false;
            list.HorizontalScrollbar = true;
            list.BackColor = Color.White;
            list.BorderStyle = BorderStyle.FixedSingle;
        }

        private void PopulateFilters()
        {
            categoryFilter.Items.Add("All categories");
            foreach (string category in repository.Categories) categoryFilter.Items.Add(category);
            categoryFilter.SelectedIndex = 0;
            typeFilter.Items.Add("All types");
            typeFilter.Items.Add("Events only");
            typeFilter.Items.Add("Announcements only");
            typeFilter.SelectedIndex = 0;

            DateChoice anyDate = new DateChoice(null, "Any date");
            startDateFilter.Items.Add(anyDate);
            endDateFilter.Items.Add(anyDate);
            foreach (DateTime date in repository.EventDates)
            {
                DateChoice choice = new DateChoice(date, date.ToString("ddd, dd MMM yyyy"));
                startDateFilter.Items.Add(choice);
                endDateFilter.Items.Add(choice);
            }
            startDateFilter.SelectedIndex = 0;
            endDateFilter.SelectedIndex = 0;
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated) RefreshSearch(false);
        }

        private void ClearFilters(object sender, EventArgs e)
        {
            searchBox.Clear();
            categoryFilter.SelectedIndex = 0;
            startDateFilter.SelectedIndex = 0;
            endDateFilter.SelectedIndex = 0;
            typeFilter.SelectedIndex = 0;
            RefreshSearch(false);
        }

        private void RefreshSearch(bool recordSearch)
        {
            try
            {
                DateChoice start = startDateFilter.SelectedItem as DateChoice;
                DateChoice end = endDateFilter.SelectedItem as DateChoice;
                DateTime? startDate = start == null ? null : start.Date;
                DateTime? endDate = end == null ? null : end.Date;
                if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                {
                    resultMessage.Text = "The start date must be on or before the end date.";
                    return;
                }

                string category = categoryFilter.SelectedIndex > 0 ? categoryFilter.SelectedItem as string : null;
                MunicipalEventType? eventType = typeFilter.SelectedIndex == 1 ? MunicipalEventType.Event
                    : typeFilter.SelectedIndex == 2 ? MunicipalEventType.Announcement : (MunicipalEventType?)null;
                IList<Event> matches = recordSearch
                    ? searchService.Search(searchBox.Text, category, startDate, endDate, eventType)
                    : searchService.Preview(searchBox.Text, category, startDate, endDate, eventType);

                resultsGrid.Rows.Clear();
                foreach (Event item in matches)
                {
                    int row = resultsGrid.Rows.Add(item.Title, item.Category, item.Date.ToString("ddd, dd MMM yyyy HH:mm"), item.Location);
                    resultsGrid.Rows[row].Tag = item.Id;
                }
                resultMessage.Text = matches.Count == 0 ? "No matching events or announcements. Try changing or clearing your filters." : matches.Count + " item(s) found.";
                RefreshActivityLists();
            }
            catch (ArgumentException exception)
            {
                resultMessage.Text = exception.Message;
            }
        }

        private void RefreshActivityLists()
        {
            recommendationsList.Items.Clear();
            foreach (EventRecommendation recommendation in recommendationService.Recommend(currentEventId, 5))
                recommendationsList.Items.Add(recommendation.Event.Title + " — " + recommendation.Reason);

            searchesList.Items.Clear();
            foreach (SearchRecord record in searchService.RecentSearches)
                searchesList.Items.Add(new SearchListItem(record));

            recentlyViewedList.Items.Clear();
            foreach (string id in activityTracker.RecentEventIds)
            {
                Event item;
                if (repository.TryGetById(id, out item)) recentlyViewedList.Items.Add(item.Title + " — " + item.Date.ToString("dd MMM yyyy"));
            }
        }

        private void ViewSelectedResult()
        {
            if (resultsGrid.SelectedRows.Count == 0 || resultsGrid.SelectedRows[0].Tag == null)
            {
                MessageBox.Show(this, "Select an event or announcement first.", "View details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ShowEventDetails(resultsGrid.SelectedRows[0].Tag.ToString());
        }

        private void ViewSelectedRecommendation()
        {
            int index = recommendationsList.SelectedIndex;
            IList<EventRecommendation> recommendations = recommendationService.Recommend(currentEventId, 5);
            if (index >= 0 && index < recommendations.Count) ShowEventDetails(recommendations[index].Event.Id);
        }

        private void ViewSelectedRecent()
        {
            int index = recentlyViewedList.SelectedIndex;
            IList<string> ids = activityTracker.RecentEventIds;
            if (index >= 0 && index < ids.Count) ShowEventDetails(ids[index]);
        }

        private void ShowEventDetails(string id)
        {
            Event item;
            if (!repository.TryGetById(id, out item))
            {
                MessageBox.Show(this, "This event is no longer available.", "Event details", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            currentEventId = item.Id;
            activityTracker.RecordViewed(item.Id);
            string end = item.EndDateTime.HasValue ? " to " + item.EndDateTime.Value.ToString("HH:mm") : string.Empty;
            string details = item.Title + "\n\n" + item.Description + "\n\n"
                + "Category: " + item.Category + "\n"
                + "Type: " + (item.Type == MunicipalEventType.Event ? "Event" : "Announcement") + "\n"
                + "Date and time: " + item.Date.ToString("dddd, dd MMMM yyyy HH:mm") + end + "\n"
                + "Location: " + item.Location + "\n"
                + "Importance: " + item.Importance + " of 5\n"
                + "Tags: " + string.Join(", ", item.Tags);
            MessageBox.Show(this, details, "Event details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshActivityLists();
        }

        private void RepeatSelectedSearch()
        {
            SearchListItem selected = searchesList.SelectedItem as SearchListItem;
            if (selected == null) return;
            SearchRecord record = selected.Record;
            searchBox.Text = record.SearchTerms;
            SelectCategory(record.Category);
            SelectDate(startDateFilter, record.StartDate);
            SelectDate(endDateFilter, record.EndDate);
            typeFilter.SelectedIndex = !record.EventType.HasValue ? 0 : record.EventType.Value == MunicipalEventType.Event ? 1 : 2;
            IList<Event> matches = searchService.Repeat(record);
            DisplayMatches(matches);
            RefreshActivityLists();
        }

        private void SelectCategory(string category)
        {
            int index = 0;
            if (!string.IsNullOrWhiteSpace(category))
                for (int i = 1; i < categoryFilter.Items.Count; i++)
                    if (string.Equals(categoryFilter.Items[i] as string, category, StringComparison.OrdinalIgnoreCase)) { index = i; break; }
            categoryFilter.SelectedIndex = index;
        }

        private static void SelectDate(ComboBox combo, DateTime? date)
        {
            int index = 0;
            if (date.HasValue)
                for (int i = 1; i < combo.Items.Count; i++)
                {
                    DateChoice choice = combo.Items[i] as DateChoice;
                    if (choice != null && choice.Date == date.Value.Date) { index = i; break; }
                }
            combo.SelectedIndex = index;
        }

        private void DisplayMatches(IList<Event> matches)
        {
            resultsGrid.Rows.Clear();
            foreach (Event item in matches)
            {
                int row = resultsGrid.Rows.Add(item.Title, item.Category, item.Date.ToString("ddd, dd MMM yyyy HH:mm"), item.Location);
                resultsGrid.Rows[row].Tag = item.Id;
            }
            resultMessage.Text = matches.Count == 0 ? "No matching events or announcements." : matches.Count + " item(s) found.";
        }

        private static Label CreateHeading(string text)
        {
            return new Label { Text = text, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(18, 53, 91), TextAlign = ContentAlignment.MiddleLeft };
        }

        private static Button CreateButton(string text, Color color)
        {
            return new Button { Text = text, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), UseVisualStyleBackColor = false, Height = 32 };
        }

        private sealed class SearchListItem
        {
            public SearchRecord Record { get; private set; }
            public SearchListItem(SearchRecord record) { Record = record; }
            public override string ToString()
            {
                string filters = string.IsNullOrWhiteSpace(Record.SearchTerms) ? "All items" : "\"" + Record.SearchTerms + "\"";
                if (!string.IsNullOrWhiteSpace(Record.Category)) filters += " · " + Record.Category;
                if (Record.StartDate.HasValue || Record.EndDate.HasValue)
                    filters += " · " + (Record.StartDate.HasValue ? Record.StartDate.Value.ToString("dd MMM") : "Any") + "–" + (Record.EndDate.HasValue ? Record.EndDate.Value.ToString("dd MMM") : "Any");
                return Record.SearchedAt.ToString("HH:mm") + "  " + filters;
            }
        }
    }
}
