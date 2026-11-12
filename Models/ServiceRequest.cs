using System;
using System.Collections.Generic;

namespace MyMzansi.Models
{
    public enum RequestStatus
    {
        Pending,
        InProgress,
        Completed,
        Rejected
    }

    public enum RequestPriority
    {
        Critical,
        High,
        Medium,
        Low
    }

    public sealed class RequestStatusUpdate
    {
        public RequestStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Note { get; set; }

        public RequestStatusUpdate()
        {
        }

        public RequestStatusUpdate(RequestStatus status, DateTime updatedAt, string note)
        {
            Status = status;
            UpdatedAt = updatedAt;
            Note = note ?? string.Empty;
        }
    }

    public sealed class ServiceRequest
    {
        public string RequestId { get; set; }
        public string ResidentName { get; set; }
        public string ContactInformation { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime SubmissionDate { get; set; }
        public RequestPriority Priority { get; set; }
        public string AssignedDepartment { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<RequestStatusUpdate> StatusHistory { get; set; }
        public bool IsDemonstrationData { get; set; }

        public ServiceRequest()
        {
            StatusHistory = new List<RequestStatusUpdate>();
        }

        public void RecordStatus(RequestStatus status, DateTime updatedAt, string note)
        {
            Status = status;
            LastUpdated = updatedAt;
            if (StatusHistory == null) StatusHistory = new List<RequestStatusUpdate>();
            StatusHistory.Add(new RequestStatusUpdate(status, updatedAt, note));
        }
    }
}
