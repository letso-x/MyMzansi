using System;
using System.Collections.Generic;

namespace MyMzansi.DataStructures
{
    public sealed class BinaryMinHeap<T>
    {
        private readonly List<T> items = new List<T>();
        private readonly IComparer<T> comparer;
        public int Count { get { return items.Count; } }
        public bool IsEmpty { get { return items.Count == 0; } }

        public BinaryMinHeap(IComparer<T> itemComparer) { comparer = itemComparer ?? Comparer<T>.Default; }
        public BinaryMinHeap() : this(null) { }

        /// <summary>Adds an item and restores the min-heap property.</summary>
        public void Insert(T item)
        {
            items.Add(item);
            SiftUp(items.Count - 1);
        }

        /// <summary>Returns the smallest item without removing it.</summary>
        public T Peek()
        {
            if (items.Count == 0) throw new InvalidOperationException("The heap is empty.");
            return items[0];
        }

        /// <summary>Removes and returns the smallest item.</summary>
        public T ExtractMin()
        {
            if (items.Count == 0) throw new InvalidOperationException("The heap is empty.");
            T minimum = items[0];
            int last = items.Count - 1;
            items[0] = items[last];
            items.RemoveAt(last);
            if (items.Count > 0) SiftDown(0);
            return minimum;
        }

        /// <summary>Replaces contents and restores heap order bottom-up in O(n).</summary>
        public void Heapify(IEnumerable<T> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            items.Clear();
            items.AddRange(values);
            for (int i = items.Count / 2 - 1; i >= 0; i--) SiftDown(i);
        }

        /// <summary>Returns items in ascending comparer order using repeated minimum extraction.</summary>
        public IList<T> HeapSort()
        {
            BinaryMinHeap<T> copy = new BinaryMinHeap<T>(comparer);
            copy.Heapify(items);
            List<T> sorted = new List<T>(items.Count);
            while (!copy.IsEmpty) sorted.Add(copy.ExtractMin());
            return sorted.AsReadOnly();
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (comparer.Compare(items[index], items[parent]) >= 0) break;
                Swap(index, parent);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                int left = index * 2 + 1;
                int right = left + 1;
                int smallest = index;
                if (left < items.Count && comparer.Compare(items[left], items[smallest]) < 0) smallest = left;
                if (right < items.Count && comparer.Compare(items[right], items[smallest]) < 0) smallest = right;
                if (smallest == index) return;
                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int first, int second)
        {
            T value = items[first]; items[first] = items[second]; items[second] = value;
        }
    }
}
