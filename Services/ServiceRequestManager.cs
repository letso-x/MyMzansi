using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyMzansi.Data;
using MyMzansi.DataStructures;
using MyMzansi.Models;

namespace MyMzansi.Services
{
    public sealed class ServiceRequestManager
    {
        private readonly Dictionary<string, ServiceRequest> requestsById;
        private readonly Dictionary<Issue, string> importedIssues;
        private readonly ServiceRequestRepository repository;
        private int nextRequestNumber;
        private bool persistenceAvailable;

        public string PersistenceWarning { get; private set; }
        public event EventHandler RequestsChanged;
        public string StoragePath { get { return repository.FilePath; } }
        public BinarySearchTree<string, ServiceRequest> RequestIdSearchTree { get; private set; }
        public AVLTree<string, ServiceRequest> RequestIdAvlTree { get; private set; }
        public RedBlackTree<string, ServiceRequest> RequestIdRedBlackTree { get; private set; }
        public BinaryTree<ServiceRequest> RequestBinaryTree { get; private set; }
        public GeneralTree<string> CategoryHierarchy { get; private set; }
        public ServiceRequestPriorityQueue PriorityQueue { get; private set; }
        public Graph<string> ServiceRelationshipGraph { get; private set; }
        public WeightedGraph<string> DemonstrationDepartmentNetwork { get; private set; }
        public MinimumSpanningTreeResult<string> DepartmentNetworkMst { get; private set; }
        public IList<ServiceRequest> Requests { get { return RequestBinaryTree == null ? requestsById.Values.ToList().AsReadOnly() : RequestBinaryTree.PreOrderTraversal(); } }

        public ServiceRequestManager()
            : this(new ServiceRequestRepository())
        {
        }

        public ServiceRequestManager(ServiceRequestRepository requestRepository)
        {
            if (requestRepository == null) throw new ArgumentNullException("requestRepository");
            repository = requestRepository;
            requestsById = new Dictionary<string, ServiceRequest>(StringComparer.OrdinalIgnoreCase);
            importedIssues = new Dictionary<Issue, string>();
            nextRequestNumber = 1001;
            persistenceAvailable = true;

            string loadError;
            IList<ServiceRequest> loaded = repository.Load(out loadError);
            if (!string.IsNullOrEmpty(loadError))
            {
                PersistenceWarning = loadError + " Changes will remain in memory until the data file is repaired.";
                persistenceAvailable = false;
            }
            else
            {
                foreach (ServiceRequest request in loaded)
                {
                    if (request == null || string.IsNullOrWhiteSpace(request.RequestId) || requestsById.ContainsKey(request.RequestId)) continue;
                    if (request.StatusHistory == null) request.StatusHistory = new List<RequestStatusUpdate>();
                    requestsById.Add(request.RequestId.Trim(), request);
                    UpdateNextNumber(request.RequestId);
                }
                if (requestsById.Count == 0) AddDemonstrationRequests();
            }
            RebuildIndexes();
        }

        public ServiceRequest AddRequest(ServiceRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (string.IsNullOrWhiteSpace(request.RequestId)) request.RequestId = GenerateRequestId();
            request.RequestId = request.RequestId.Trim();
            if (requestsById.ContainsKey(request.RequestId)) throw new ArgumentException("A request with this ID already exists.", "request");
            if (string.IsNullOrWhiteSpace(request.Category)) throw new ArgumentException("A service category is required.", "request");
            if (string.IsNullOrWhiteSpace(request.Description)) throw new ArgumentException("A request description is required.", "request");
            if (string.IsNullOrWhiteSpace(request.AssignedDepartment)) request.AssignedDepartment = DepartmentFor(request.Category);
            if (request.SubmissionDate == DateTime.MinValue) request.SubmissionDate = DateTime.Now;
            if (request.LastUpdated == DateTime.MinValue) request.LastUpdated = request.SubmissionDate;
            if (request.StatusHistory == null) request.StatusHistory = new List<RequestStatusUpdate>();
            if (request.StatusHistory.Count == 0) request.StatusHistory.Add(new RequestStatusUpdate(request.Status, request.SubmissionDate, "Request submitted."));
            requestsById.Add(request.RequestId, request);
            UpdateNextNumber(request.RequestId);
            RebuildIndexes();
            OnRequestsChanged();
            SaveChanges();
            return request;
        }

        public bool TryGetRequest(string requestId, out ServiceRequest request)
        {
            request = null;
            if (string.IsNullOrWhiteSpace(requestId)) return false;
            string key = requestId.Trim();
            ServiceRequest binarySearchResult = null;
            ServiceRequest avlResult = null;
            ServiceRequest redBlackResult = null;
            ServiceRequest binaryTreeResult = RequestBinaryTree == null ? null : RequestBinaryTree.Find(item => string.Equals(item.RequestId, key, StringComparison.OrdinalIgnoreCase));
            bool found = RequestIdSearchTree != null && RequestIdSearchTree.TryGetValue(key, out binarySearchResult);
            bool foundAvl = RequestIdAvlTree != null && RequestIdAvlTree.TryGetValue(key, out avlResult);
            bool foundRedBlack = RequestIdRedBlackTree != null && RequestIdRedBlackTree.TryGetValue(key, out redBlackResult);
            if (found && foundAvl && foundRedBlack && ReferenceEquals(binarySearchResult, avlResult) &&
                ReferenceEquals(binarySearchResult, redBlackResult) && ReferenceEquals(binarySearchResult, binaryTreeResult))
            {
                request = binarySearchResult;
                return true;
            }
            return requestsById.TryGetValue(key, out request);
        }

        public bool UpdateStatus(string requestId, RequestStatus status, string note)
        {
            ServiceRequest request;
            if (!TryGetRequest(requestId, out request)) return false;
            request.RecordStatus(status, DateTime.Now, string.IsNullOrWhiteSpace(note) ? "Status updated." : note.Trim());
            RebuildIndexes();
            OnRequestsChanged();
            SaveChanges();
            return true;
        }

        public bool UpdatePriority(string requestId, RequestPriority priority)
        {
            ServiceRequest request;
            if (!TryGetRequest(requestId, out request)) return false;
            request.Priority = priority;
            request.LastUpdated = DateTime.Now;
            RebuildIndexes();
            OnRequestsChanged();
            SaveChanges();
            return true;
        }

        public bool DeleteRequest(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId) || !requestsById.Remove(requestId.Trim())) return false;
            RebuildIndexes();
            OnRequestsChanged();
            SaveChanges();
            return true;
        }

        private void OnRequestsChanged()
        {
            EventHandler handler = RequestsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void RebuildIndexes()
        {
            List<ServiceRequest> ordered = requestsById.Values.OrderBy(request => request.RequestId, StringComparer.OrdinalIgnoreCase).ToList();
            RequestIdSearchTree = new BinarySearchTree<string, ServiceRequest>(StringComparer.OrdinalIgnoreCase);
            RequestIdAvlTree = new AVLTree<string, ServiceRequest>(StringComparer.OrdinalIgnoreCase);
            RequestIdRedBlackTree = new RedBlackTree<string, ServiceRequest>(StringComparer.OrdinalIgnoreCase);
            RequestBinaryTree = new BinaryTree<ServiceRequest>();
            CategoryHierarchy = new GeneralTree<string>(StringComparer.OrdinalIgnoreCase);
            PriorityQueue = new ServiceRequestPriorityQueue();
            ServiceRelationshipGraph = new Graph<string>(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
            CategoryHierarchy.Add(null, "Municipal Services");
            ServiceRelationshipGraph.AddVertex("Municipal Services");
            InsertBalancedSearchTree(ordered, 0, ordered.Count - 1);

            foreach (ServiceRequest request in ordered)
            {
                RequestIdAvlTree.Insert(request.RequestId, request);
                RequestIdRedBlackTree.Insert(request.RequestId, request);
                RequestBinaryTree.Insert(request);
                if (!string.IsNullOrWhiteSpace(request.AssignedDepartment))
                {
                    if (CategoryHierarchy.Find(request.AssignedDepartment) == null) CategoryHierarchy.Add("Municipal Services", request.AssignedDepartment);
                    ServiceRelationshipGraph.AddVertex(request.AssignedDepartment);
                    ServiceRelationshipGraph.AddEdge("Municipal Services", request.AssignedDepartment);
                    if (!string.IsNullOrWhiteSpace(request.Category) && CategoryHierarchy.Find(request.Category) == null)
                        CategoryHierarchy.Add(request.AssignedDepartment, request.Category);
                    if (!string.IsNullOrWhiteSpace(request.Category))
                    {
                        ServiceRelationshipGraph.AddVertex(request.Category);
                        ServiceRelationshipGraph.AddEdge(request.AssignedDepartment, request.Category);
                        ServiceRelationshipGraph.AddVertex(request.RequestId);
                        ServiceRelationshipGraph.AddEdge(request.Category, request.RequestId);
                    }
                }
            }
            PriorityQueue.Rebuild(ordered);
            DemonstrationDepartmentNetwork = CreateDemonstrationDepartmentNetwork();
            DepartmentNetworkMst = KruskalMst.Find(DemonstrationDepartmentNetwork, StringComparer.OrdinalIgnoreCase);
            if (!RequestIdRedBlackTree.IsValid()) throw new InvalidOperationException("The service request red-black index failed its invariant check.");
        }

        private static WeightedGraph<string> CreateDemonstrationDepartmentNetwork()
        {
            WeightedGraph<string> network = new WeightedGraph<string>(StringComparer.OrdinalIgnoreCase);
            network.AddVertex("Municipal Services");
            network.AddVertex("Water Services");
            network.AddVertex("Electricity Services");
            network.AddVertex("Roads and Transport");
            network.AddVertex("Waste Management");
            network.AddEdge("Municipal Services", "Water Services", 4);
            network.AddEdge("Municipal Services", "Electricity Services", 6);
            network.AddEdge("Municipal Services", "Roads and Transport", 5);
            network.AddEdge("Municipal Services", "Waste Management", 3);
            network.AddEdge("Water Services", "Roads and Transport", 7);
            network.AddEdge("Electricity Services", "Roads and Transport", 2);
            network.AddEdge("Roads and Transport", "Waste Management", 4);
            return network;
        }

        private void InsertBalancedSearchTree(IList<ServiceRequest> ordered, int first, int last)
        {
            if (first > last) return;
            int middle = first + (last - first) / 2;
            ServiceRequest request = ordered[middle];
            RequestIdSearchTree.Insert(request.RequestId, request);
            InsertBalancedSearchTree(ordered, first, middle - 1);
            InsertBalancedSearchTree(ordered, middle + 1, last);
        }

        public IList<ServiceRequest> ImportIssue(Issue issue)
        {
            if (issue == null) throw new ArgumentNullException("issue");
            string existingId;
            if (importedIssues.TryGetValue(issue, out existingId)) return new List<ServiceRequest> { requestsById[existingId] }.AsReadOnly();

            string category = issue.Category == null || string.IsNullOrWhiteSpace(issue.Category.Name) ? "General" : issue.Category.Name.Trim();
            string description = issue.Description ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(issue.Subject)) description = issue.Subject.Trim() + Environment.NewLine + description;
            if (!string.IsNullOrWhiteSpace(issue.Location)) description += Environment.NewLine + "Location: " + issue.Location.Trim();
            ServiceRequest request = new ServiceRequest
            {
                RequestId = GenerateRequestId(),
                ResidentName = "Not provided",
                ContactInformation = "Not provided",
                Category = category,
                Description = description.Trim(),
                SubmissionDate = issue.DateReported == DateTime.MinValue ? DateTime.Now : issue.DateReported,
                Priority = RequestPriority.Medium,
                AssignedDepartment = DepartmentFor(category),
                Status = RequestStatus.Pending,
                LastUpdated = issue.DateReported == DateTime.MinValue ? DateTime.Now : issue.DateReported,
                IsDemonstrationData = false
            };
            try { AddRequest(request); }
            catch
            {
                if (requestsById.ContainsKey(request.RequestId)) importedIssues[issue] = request.RequestId;
                throw;
            }
            importedIssues.Add(issue, request.RequestId);
            return new List<ServiceRequest> { request }.AsReadOnly();
        }

        public IList<ServiceRequest> GetOutstandingByPriority()
        {
            return Requests.Where(request => request.Status == RequestStatus.Pending || request.Status == RequestStatus.InProgress)
                .OrderBy(request => request.Priority)
                .ThenBy(request => request.SubmissionDate)
                .ThenBy(request => request.RequestId, StringComparer.OrdinalIgnoreCase)
                .ToList().AsReadOnly();
        }

        public void SaveChanges()
        {
            if (!persistenceAvailable) return;
            try
            {
                repository.Save(requestsById.Values);
                PersistenceWarning = null;
            }
            catch (Exception exception)
            {
                PersistenceWarning = "Changes could not be saved: " + exception.Message;
                throw;
            }
        }

        private string GenerateRequestId()
        {
            string id;
            do { id = "SR-" + nextRequestNumber.ToString("D4", CultureInfo.InvariantCulture); nextRequestNumber++; }
            while (requestsById.ContainsKey(id));
            return id;
        }

        private void UpdateNextNumber(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId)) return;
            string[] parts = requestId.Split('-');
            int parsed;
            if (parts.Length > 1 && int.TryParse(parts[parts.Length - 1], out parsed) && parsed >= nextRequestNumber) nextRequestNumber = parsed + 1;
        }

        private void AddDemonstrationRequests()
        {
            DateTime now = DateTime.Now;
            AddDemo("SR-1001", "Water leak", "Demonstration record: reported water leak near a public road.", RequestPriority.Critical, RequestStatus.InProgress, "Water Services", now.AddDays(-2));
            AddDemo("SR-1002", "Potholes & Resurfacing", "Demonstration record: road surface requires inspection.", RequestPriority.High, RequestStatus.Pending, "Roads and Transport", now.AddDays(-1));
            AddDemo("SR-1003", "Missed Collection", "Demonstration record: household waste collection follow-up.", RequestPriority.Medium, RequestStatus.Completed, "Waste Management", now.AddDays(-4));
            AddDemo("SR-1004", "Streetlight Outages", "Demonstration record: streetlight fault submitted for review.", RequestPriority.Low, RequestStatus.Rejected, "Electricity Services", now.AddDays(-3));
            AddDemo("SR-1005", "Sewer Overflows", "Demonstration record: sanitation issue pending assessment.", RequestPriority.High, RequestStatus.Pending, "Water Services", now);
            try { SaveChanges(); }
            catch { }
        }

        private void AddDemo(string id, string category, string description, RequestPriority priority, RequestStatus status, string department, DateTime submitted)
        {
            ServiceRequest request = new ServiceRequest
            {
                RequestId = id,
                ResidentName = "Demonstration resident",
                ContactInformation = "Demonstration data",
                Category = category,
                Description = description,
                SubmissionDate = submitted,
                Priority = priority,
                AssignedDepartment = department,
                Status = status,
                LastUpdated = status == RequestStatus.InProgress ? submitted.AddHours(4) : submitted,
                IsDemonstrationData = true
            };
            request.StatusHistory.Add(new RequestStatusUpdate(RequestStatus.Pending, submitted, "Demonstration record; not a verified municipal update."));
            if (status != RequestStatus.Pending) request.StatusHistory.Add(new RequestStatusUpdate(status, request.LastUpdated, "Demonstration status; not a verified municipal update."));
            requestsById.Add(id, request);
            UpdateNextNumber(id);
        }

        private static string DepartmentFor(string category)
        {
            string value = (category ?? string.Empty).ToLowerInvariant();
            if (value.Contains("water") || value.Contains("sewer")) return "Water Services";
            if (value.Contains("electric") || value.Contains("streetlight") || value.Contains("traffic")) return "Electricity Services";
            if (value.Contains("road") || value.Contains("pothole")) return "Roads and Transport";
            if (value.Contains("waste") || value.Contains("dump")) return "Waste Management";
            return "Municipal Services";
        }
    }
}
