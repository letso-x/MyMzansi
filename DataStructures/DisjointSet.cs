using System;
using System.Collections.Generic;

namespace MyMzansi.DataStructures
{
    public sealed class DisjointSet<T>
    {
        private readonly Dictionary<T, T> parent;
        private readonly Dictionary<T, int> rank;
        public int Count { get { return parent.Count; } }

        public DisjointSet(IEnumerable<T> elements, IEqualityComparer<T> equalityComparer)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            parent = new Dictionary<T, T>(equalityComparer ?? EqualityComparer<T>.Default);
            rank = new Dictionary<T, int>(parent.Comparer);
            foreach (T element in elements)
            {
                if (parent.ContainsKey(element)) continue;
                parent.Add(element, element); rank.Add(element, 0);
            }
        }
        public DisjointSet(IEnumerable<T> elements) : this(elements, null) { }

        /// <summary>Finds a representative and compresses the traversed path.</summary>
        public T Find(T item)
        {
            T parentItem;
            if (!parent.TryGetValue(item, out parentItem)) throw new ArgumentException("The item is not in this disjoint set.", "item");
            if (!parent.Comparer.Equals(item, parentItem)) parent[item] = Find(parentItem);
            return parent[item];
        }

        /// <summary>Unites two components by rank. Returns false if they already share a component.</summary>
        public bool Union(T first, T second)
        {
            T firstRoot = Find(first);
            T secondRoot = Find(second);
            if (parent.Comparer.Equals(firstRoot, secondRoot)) return false;
            int firstRank = rank[firstRoot];
            int secondRank = rank[secondRoot];
            if (firstRank < secondRank) parent[firstRoot] = secondRoot;
            else if (firstRank > secondRank) parent[secondRoot] = firstRoot;
            else { parent[secondRoot] = firstRoot; rank[firstRoot]++; }
            return true;
        }
    }
}
