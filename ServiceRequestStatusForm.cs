using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MyMzansi.Models;
using MyMzansi.Services;

namespace MyMzansi
{
    public sealed class ServiceRequestStatusForm : Form
    {
        private sealed class RequestGridRow
        {
            public string RequestId { get; set; }
            public string ResidentName { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public DateTime SubmissionDate { get; set; }
            public string Priority { get; set; }
            public string AssignedDepartment { get; set; }
            public string Status { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        private readonly ServiceRequestManager manager;
        private readonly IList<Issue> issueReports;
        private readonly DataGridView requestsGrid = new DataGridView();
        private readonly TextBox requestIdBox = new TextBox();
        private readonly ComboBox statusFilter = new ComboBox();
        private readonly ComboBox sortBy = new ComboBox();
        private readonly ComboBox sortDirection = new ComboBox();
        private readonly Label messageLabel = new Label();
        private readonly Label totalValue = new Label();
        private readonly Label pendingValue = new Label();
        private readonly Label inProgressValue = new Label();
        private readonly Label completedValue = new Label();
        private readonly Label rejectedValue = new Label();
        private readonly Label selectedStatus = new Label();
        private readonly TextBox detailsText = new TextBox();
        private readonly ListBox historyList = new ListBox();
        private readonly ListBox priorityList = new ListBox();
        private readonly ListBox networkList = new ListBox();
        private string selectedRequestId;

        public ServiceRequestStatusForm()
            : this(new ServiceRequestManager(), new List<Issue>())
        {
        }

        public ServiceRequestStatusForm(ServiceRequestManager requestManager, IEnumerable<Issue> issues)
        {
            if (requestManager == null) throw new ArgumentNullException("requestManager");
            manager = requestManager;
            issueReports = (issues ?? Enumerable.Empty<Issue>()).Where(issue => issue != null).ToList().AsReadOnly();
            Text = "Service Request Status - MyMzansi";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(1080, 680);
            Size = new Size(1440, 900);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F);
            BuildInterface();
            manager.RequestsChanged += ManagerRequestsChanged;
            ImportIssueReports();
            RefreshRequests(null);
            if (!string.IsNullOrWhiteSpace(manager.PersistenceWarning))
                messageLabel.Text = manager.PersistenceWarning;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            manager.RequestsChanged -= ManagerRequestsChanged;
            base.OnFormClosed(e);
        }

        private void ManagerRequestsChanged(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate { RefreshRequests(selectedRequestId); }));
                return;
            }
            RefreshRequests(selectedRequestId);
        }

        private void BuildInterface()
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(16),
                BackColor = BackColor
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(layout);

            Panel header = new Panel { Dock = DockStyle.Fill };
            Label title = new Label
            {
                Text = "Service Request Status",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 53, 91),
                AutoSize = true,
                Location = new Point(0, 0)
            };
            Label subtitle = new Label
            {
                Text = "Track submitted municipal service requests by unique request ID.",
                ForeColor = Color.FromArgb(75, 85, 99),
                AutoSize = true,
                Location = new Point(2, 39)
            };
            Button backButton = CreateButton("Back to menu", Color.FromArgb(18, 53, 91));
            backButton.Size = new Size(130, 36);
            backButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            backButton.Location = new Point(Width - 190, 8);
            backButton.Click += delegate { Close(); };
            header.Resize += delegate { backButton.Location = new Point(header.ClientSize.Width - backButton.Width, 8); };
            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            header.Controls.Add(backButton);
            layout.Controls.Add(header, 0, 0);

            TableLayoutPanel cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 1, BackColor = BackColor };
            for (int i = 0; i < 5; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            cards.Controls.Add(CreateSummaryCard("Total requests", totalValue, Color.FromArgb(18, 53, 91)), 0, 0);
            cards.Controls.Add(CreateSummaryCard("Pending", pendingValue, Color.FromArgb(217, 119, 6)), 1, 0);
            cards.Controls.Add(CreateSummaryCard("In progress", inProgressValue, Color.FromArgb(37, 99, 235)), 2, 0);
            cards.Controls.Add(CreateSummaryCard("Completed", completedValue, Color.FromArgb(22, 163, 74)), 3, 0);
            cards.Controls.Add(CreateSummaryCard("Rejected", rejectedValue, Color.FromArgb(220, 38, 38)), 4, 0);
            layout.Controls.Add(cards, 0, 1);

            FlowLayoutPanel filters = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.White,
                Padding = new Padding(10, 12, 8, 8)
            };
            requestIdBox.Width = 180;
            requestIdBox.Font = Font;
            requestIdBox.KeyDown += delegate(object sender, KeyEventArgs args)
            {
                if (args.KeyCode == Keys.Enter) { args.SuppressKeyPress = true; SearchRequest(); }
            };
            filters.Controls.Add(CreateCaption("Request ID"));
            filters.Controls.Add(requestIdBox);
            Button searchButton = CreateButton("Search", Color.FromArgb(18, 53, 91));
            searchButton.Click += delegate { SearchRequest(); };
            filters.Controls.Add(searchButton);
            Button clearButton = CreateButton("Clear", Color.FromArgb(107, 114, 128));
            clearButton.Click += delegate { requestIdBox.Clear(); statusFilter.SelectedIndex = 0; messageLabel.Text = string.Empty; RefreshRequests(null); };
            filters.Controls.Add(clearButton);
            filters.Controls.Add(CreateCaption("Status"));
            statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            statusFilter.Width = 135;
            statusFilter.Items.AddRange(new object[] { "All Requests", "Pending", "In Progress", "Completed", "Rejected" });
            statusFilter.SelectedIndex = 0;
            statusFilter.SelectedIndexChanged += delegate { RefreshRequests(null); };
            filters.Controls.Add(statusFilter);
            filters.Controls.Add(CreateCaption("Sort by"));
            sortBy.DropDownStyle = ComboBoxStyle.DropDownList;
            sortBy.Width = 135;
            sortBy.Items.AddRange(new object[] { "Request ID", "Submission Date", "Priority", "Status" });
            sortBy.SelectedIndex = 0;
            sortBy.SelectedIndexChanged += delegate { RefreshRequests(selectedRequestId); };
            filters.Controls.Add(sortBy);
            sortDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            sortDirection.Width = 105;
            sortDirection.Items.AddRange(new object[] { "Ascending", "Descending" });
            sortDirection.SelectedIndex = 0;
            sortDirection.SelectedIndexChanged += delegate { RefreshRequests(selectedRequestId); };
            filters.Controls.Add(sortDirection);
            Button refreshButton = CreateButton("Refresh", Color.FromArgb(55, 65, 81));
            refreshButton.Click += delegate { RefreshRequests(selectedRequestId); };
            filters.Controls.Add(refreshButton);
            layout.Controls.Add(filters, 0, 2);

            SplitContainer content = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                Size = new Size(1400, 600),
                SplitterDistance = 850,
                FixedPanel = FixedPanel.Panel2,
                Panel1MinSize = 460,
                Panel2MinSize = 310,
                BackColor = BackColor
            };
            layout.Controls.Add(content, 0, 3);
            BuildRequestGrid(content.Panel1);
            BuildDetailsPanel(content.Panel2);

            messageLabel.Dock = DockStyle.Bottom;
            messageLabel.Height = 24;
            messageLabel.ForeColor = Color.FromArgb(153, 27, 27);
            messageLabel.Padding = new Padding(4, 3, 4, 0);
            Controls.Add(messageLabel);
        }

        private void BuildRequestGrid(Control parent)
        {
            requestsGrid.Dock = DockStyle.Fill;
            requestsGrid.ReadOnly = true;
            requestsGrid.AllowUserToAddRows = false;
            requestsGrid.AllowUserToDeleteRows = false;
            requestsGrid.AllowUserToResizeRows = false;
            requestsGrid.MultiSelect = false;
            requestsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            requestsGrid.AutoGenerateColumns = false;
            requestsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            requestsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            requestsGrid.RowHeadersVisible = false;
            requestsGrid.BackgroundColor = Color.White;
            requestsGrid.BorderStyle = BorderStyle.None;
            requestsGrid.GridColor = Color.FromArgb(226, 232, 240);
            requestsGrid.ColumnHeadersHeight = 38;
            requestsGrid.EnableHeadersVisualStyles = false;
            requestsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(18, 53, 91);
            requestsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            requestsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            requestsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            requestsGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(31, 41, 55);
            requestsGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            requestsGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            AddGridColumn("Request ID", "RequestId", 85);
            AddGridColumn("Resident", "ResidentName", 100);
            AddGridColumn("Category", "Category", 100);
            AddGridColumn("Description", "Description", 180);
            AddGridColumn("Submitted", "SubmissionDate", 95, "g");
            AddGridColumn("Priority", "Priority", 65);
            AddGridColumn("Department", "AssignedDepartment", 105);
            AddGridColumn("Status", "Status", 75);
            AddGridColumn("Last updated", "LastUpdated", 95, "g");
            requestsGrid.CellFormatting += FormatStatusCell;
            requestsGrid.SelectionChanged += delegate
            {
                if (requestsGrid.CurrentRow == null || requestsGrid.CurrentRow.DataBoundItem == null) return;
                RequestGridRow row = requestsGrid.CurrentRow.DataBoundItem as RequestGridRow;
                if (row != null) ShowRequestDetails(row.RequestId);
            };
            parent.Controls.Add(requestsGrid);
        }

        private void BuildDetailsPanel(Control parent)
        {
            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(8, 0, 0, 0),
                AutoScroll = true,
                BackColor = BackColor
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 220));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 125));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 125));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 135));
            parent.Controls.Add(panel);

            GroupBox details = new GroupBox { Text = "Selected request", Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(8) };
            TableLayoutPanel detailsLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            selectedStatus.Dock = DockStyle.Fill;
            selectedStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            detailsText.Dock = DockStyle.Fill;
            detailsText.Multiline = true;
            detailsText.ReadOnly = true;
            detailsText.ScrollBars = ScrollBars.Vertical;
            detailsText.BorderStyle = BorderStyle.None;
            detailsText.BackColor = Color.White;
            detailsLayout.Controls.Add(selectedStatus, 0, 0);
            detailsLayout.Controls.Add(detailsText, 0, 1);
            details.Controls.Add(detailsLayout);
            panel.Controls.Add(details, 0, 0);

            GroupBox history = new GroupBox { Text = "Status update history", Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(8) };
            historyList.Dock = DockStyle.Fill;
            historyList.IntegralHeight = false;
            history.Controls.Add(historyList);
            panel.Controls.Add(history, 0, 1);

            GroupBox priority = new GroupBox { Text = "Highest-priority outstanding requests", Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(8) };
            priorityList.Dock = DockStyle.Fill;
            priorityList.IntegralHeight = false;
            priority.Controls.Add(priorityList);
            panel.Controls.Add(priority, 0, 2);

            GroupBox network = new GroupBox { Text = "Service relationships and illustrative network", Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(8) };
            networkList.Dock = DockStyle.Fill;
            networkList.IntegralHeight = false;
            networkList.HorizontalScrollbar = true;
            network.Controls.Add(networkList);
            panel.Controls.Add(network, 0, 3);
        }

        private void ImportIssueReports()
        {
            foreach (Issue issue in issueReports)
            {
                try { manager.ImportIssue(issue); }
                catch (Exception exception) { messageLabel.Text = "An issue report could not be synchronized: " + exception.Message; }
            }
        }

        private void RefreshRequests(string requestToSelect)
        {
            if (manager == null || requestsGrid.Columns.Count == 0) return;
            IEnumerable<ServiceRequest> records = manager.Requests;
            string filter = statusFilter.SelectedItem == null ? "All Requests" : statusFilter.SelectedItem.ToString();
            RequestStatus status;
            if (filter != "All Requests" && Enum.TryParse<RequestStatus>(filter.Replace(" ", string.Empty), true, out status))
                records = records.Where(request => request.Status == status);

            bool descending = sortDirection.SelectedItem != null && sortDirection.SelectedItem.ToString() == "Descending";
            string sort = sortBy.SelectedItem == null ? "Request ID" : sortBy.SelectedItem.ToString();
            if (sort == "Submission Date") records = descending ? records.OrderByDescending(request => request.SubmissionDate) : records.OrderBy(request => request.SubmissionDate);
            else if (sort == "Priority") records = descending ? records.OrderByDescending(request => request.Priority) : records.OrderBy(request => request.Priority);
            else if (sort == "Status") records = descending ? records.OrderByDescending(request => request.Status) : records.OrderBy(request => request.Status);
            else records = descending ? records.OrderByDescending(request => request.RequestId, StringComparer.OrdinalIgnoreCase) : records.OrderBy(request => request.RequestId, StringComparer.OrdinalIgnoreCase);

            List<ServiceRequest> visible = records.ToList();
            requestsGrid.DataSource = visible.Select(request => new RequestGridRow
            {
                RequestId = request.RequestId,
                ResidentName = DisplayValue(request.ResidentName),
                Category = DisplayValue(request.Category),
                Description = request.Description ?? string.Empty,
                SubmissionDate = request.SubmissionDate,
                Priority = request.Priority.ToString(),
                AssignedDepartment = DisplayValue(request.AssignedDepartment),
                Status = request.Status.ToString(),
                LastUpdated = request.LastUpdated
            }).ToList();

            IList<ServiceRequest> all = manager.Requests;
            totalValue.Text = all.Count.ToString();
            pendingValue.Text = all.Count(request => request.Status == RequestStatus.Pending).ToString();
            inProgressValue.Text = all.Count(request => request.Status == RequestStatus.InProgress).ToString();
            completedValue.Text = all.Count(request => request.Status == RequestStatus.Completed).ToString();
            rejectedValue.Text = all.Count(request => request.Status == RequestStatus.Rejected).ToString();
            PopulatePriorityQueue();
            PopulateNetworkSummary();
            if (!string.IsNullOrWhiteSpace(requestToSelect)) SelectRequestRow(requestToSelect);
            else if (requestsGrid.Rows.Count > 0) requestsGrid.Rows[0].Selected = true;
            else ShowRequestDetails(null);
        }

        private void SearchRequest()
        {
            string requestId = requestIdBox.Text.Trim();
            if (requestId.Length == 0)
            {
                messageLabel.Text = "Enter a request ID to search.";
                requestIdBox.Focus();
                return;
            }
            ServiceRequest request;
            if (!manager.TryGetRequest(requestId, out request))
            {
                messageLabel.Text = "No service request was found with ID '" + requestId + "'.";
                return;
            }
            statusFilter.SelectedIndex = 0;
            messageLabel.Text = string.Empty;
            RefreshRequests(request.RequestId);
        }

        private void SelectRequestRow(string requestId)
        {
            foreach (DataGridViewRow row in requestsGrid.Rows)
            {
                RequestGridRow item = row.DataBoundItem as RequestGridRow;
                if (item != null && string.Equals(item.RequestId, requestId, StringComparison.OrdinalIgnoreCase))
                {
                    requestsGrid.ClearSelection();
                    row.Selected = true;
                    requestsGrid.CurrentCell = row.Cells[0];
                    ShowRequestDetails(item.RequestId);
                    return;
                }
            }
            if (requestsGrid.Rows.Count > 0)
            {
                requestsGrid.Rows[0].Selected = true;
                RequestGridRow first = requestsGrid.Rows[0].DataBoundItem as RequestGridRow;
                ShowRequestDetails(first == null ? null : first.RequestId);
            }
        }

        private void ShowRequestDetails(string requestId)
        {
            ServiceRequest request;
            if (!manager.TryGetRequest(requestId, out request))
            {
                selectedRequestId = null;
                selectedStatus.Text = "Select a request to view its current status.";
                selectedStatus.ForeColor = Color.FromArgb(75, 85, 99);
                detailsText.Clear();
                historyList.Items.Clear();
                return;
            }
            selectedRequestId = request.RequestId;
            selectedStatus.Text = "CURRENT STATUS: " + request.Status.ToString().ToUpperInvariant();
            selectedStatus.ForeColor = StatusColor(request.Status);
            detailsText.Text = "Request ID: " + request.RequestId + Environment.NewLine +
                "Resident: " + DisplayValue(request.ResidentName) + Environment.NewLine +
                "Contact: " + DisplayValue(request.ContactInformation) + Environment.NewLine +
                "Category: " + DisplayValue(request.Category) + Environment.NewLine +
                "Priority: " + request.Priority + Environment.NewLine +
                "Department: " + DisplayValue(request.AssignedDepartment) + Environment.NewLine +
                "Submitted: " + request.SubmissionDate.ToString("g") + Environment.NewLine +
                "Last updated: " + request.LastUpdated.ToString("g") + Environment.NewLine +
                "Description: " + (request.Description ?? string.Empty) +
                (request.IsDemonstrationData ? Environment.NewLine + "DEMONSTRATION DATA — not a verified municipal record." : string.Empty);
            historyList.Items.Clear();
            if (request.StatusHistory != null)
            {
                foreach (RequestStatusUpdate update in request.StatusHistory.OrderBy(item => item.UpdatedAt))
                    historyList.Items.Add(update.UpdatedAt.ToString("g") + " — " + update.Status + " — " + update.Note);
            }
        }

        private void PopulatePriorityQueue()
        {
            priorityList.Items.Clear();
            foreach (ServiceRequest request in manager.PriorityQueue.GetPriorityOrder().Take(8))
                priorityList.Items.Add(request.Priority + " | " + request.RequestId + " | " + request.Category + " | " + request.Status);
            if (priorityList.Items.Count == 0) priorityList.Items.Add("No pending or in-progress requests.");
        }

        private void PopulateNetworkSummary()
        {
            networkList.Items.Clear();
            IList<string> breadthFirst = manager.ServiceRelationshipGraph.BreadthFirstSearch("Municipal Services");
            IList<string> depthFirst = manager.ServiceRelationshipGraph.DepthFirstSearch("Municipal Services");
            networkList.Items.Add("BFS: " + string.Join(" → ", breadthFirst));
            networkList.Items.Add("DFS: " + string.Join(" → ", depthFirst));
            networkList.Items.Add("MST: " + string.Join("; ", manager.DepartmentNetworkMst.Edges.Select(edge => edge.ToString())));
            networkList.Items.Add("Illustrative connection cost: " + manager.DepartmentNetworkMst.TotalWeight + " units; not verified municipal infrastructure data.");
        }

        private void AddGridColumn(string header, string property, int weight, string format = null)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = property,
                FillWeight = weight,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            if (!string.IsNullOrEmpty(format)) column.DefaultCellStyle.Format = format;
            requestsGrid.Columns.Add(column);
        }

        private void FormatStatusCell(object sender, DataGridViewCellFormattingEventArgs args)
        {
            if (requestsGrid.Columns[args.ColumnIndex].DataPropertyName != "Status" || args.Value == null) return;
            RequestStatus status;
            if (Enum.TryParse<RequestStatus>(args.Value.ToString(), out status))
            {
                args.CellStyle.ForeColor = StatusColor(status);
                args.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
        }

        private static Panel CreateSummaryCard(string title, Label value, Color accent)
        {
            Panel card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0, 0, 8, 0), Padding = new Padding(12, 7, 8, 4) };
            Panel bar = new Panel { Dock = DockStyle.Left, Width = 4, BackColor = accent };
            Label caption = new Label { Text = title, AutoSize = true, ForeColor = Color.FromArgb(75, 85, 99), Location = new Point(14, 7) };
            value.AutoSize = true;
            value.Text = "0";
            value.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            value.ForeColor = accent;
            value.Location = new Point(12, 27);
            card.Controls.Add(value);
            card.Controls.Add(caption);
            card.Controls.Add(bar);
            return card;
        }

        private static Label CreateCaption(string text)
        {
            return new Label { Text = text, AutoSize = true, Margin = new Padding(8, 9, 2, 0), ForeColor = Color.FromArgb(55, 65, 81) };
        }

        private static Button CreateButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                AutoSize = true,
                Height = 32,
                Margin = new Padding(4, 4, 6, 4),
                Padding = new Padding(8, 0, 8, 0),
                UseVisualStyleBackColor = false
            };
        }

        private static string DisplayValue(string value) { return string.IsNullOrWhiteSpace(value) ? "Not provided" : value; }

        private static Color StatusColor(RequestStatus status)
        {
            switch (status)
            {
                case RequestStatus.Pending: return Color.FromArgb(217, 119, 6);
                case RequestStatus.InProgress: return Color.FromArgb(37, 99, 235);
                case RequestStatus.Completed: return Color.FromArgb(22, 163, 74);
                case RequestStatus.Rejected: return Color.FromArgb(220, 38, 38);
                default: return Color.FromArgb(55, 65, 81);
            }
        }
    }
}
