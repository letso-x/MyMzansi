using System;
using System.Collections.Generic;
using System.Linq;
using MyMzansi.Models;

namespace MyMzansi.DataStructures
{
    public sealed class ServiceRequestPriorityQueue
    {
        private sealed class RequestComparer : IComparer<ServiceRequest>
        {
            public int Compare(ServiceRequest first, ServiceRequest second)
            {
                if (ReferenceEquals(first, second)) return 0;
                if (first == null) return 1;
                if (second == null) return -1;
                int comparison = first.Priority.CompareTo(second.Priority);
                if (comparison != 0) return comparison;
                comparison = first.SubmissionDate.CompareTo(second.SubmissionDate);
                return comparison != 0 ? comparison : string.Compare(first.RequestId, second.RequestId, StringComparison.OrdinalIgnoreCase);
            }
        }

        private readonly BinaryMinHeap<ServiceRequest> heap = new BinaryMinHeap<ServiceRequest>(new RequestComparer());
        public int Count { get { return heap.Count; } }
        public bool IsEmpty { get { return heap.IsEmpty; } }

        /// <summary>Rebuilds the queue with outstanding requests, heapifying in O(n).</summary>
        public void Rebuild(IEnumerable<ServiceRequest> requests)
        {
            if (requests == null) throw new ArgumentNullException("requests");
            heap.Heapify(requests.Where(request => request != null &&
                (request.Status == RequestStatus.Pending || request.Status == RequestStatus.InProgress)));
        }

        public ServiceRequest Peek() { return heap.Peek(); }
        public ServiceRequest Dequeue() { return heap.ExtractMin(); }
        public IList<ServiceRequest> GetPriorityOrder() { return heap.HeapSort(); }
    }
}
