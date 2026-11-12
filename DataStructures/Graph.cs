using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMzansi.DataStructures
{
    public sealed class Graph<T>
    {
        private readonly Dictionary<T, HashSet<T>> adjacency;
        private readonly IComparer<T> comparer;
        public int VertexCount { get { return adjacency.Count; } }

        public Graph(IEqualityComparer<T> equalityComparer, IComparer<T> orderComparer)
        {
            IEqualityComparer<T> equality = equalityComparer ?? EqualityComparer<T>.Default;
            comparer = orderComparer ?? Comparer<T>.Default;
            adjacency = new Dictionary<T, HashSet<T>>(equality);
        }
        public Graph() : this(null, null) { }

        /// <summary>Adds a unique vertex to the adjacency-list graph.</summary>
        public bool AddVertex(T vertex)
        {
            if (adjacency.ContainsKey(vertex)) return false;
            adjacency.Add(vertex, new HashSet<T>(adjacency.Comparer));
            return true;
        }

        /// <summary>Removes a vertex and all incident edges.</summary>
        public bool RemoveVertex(T vertex)
        {
            HashSet<T> neighbors;
            if (!adjacency.TryGetValue(vertex, out neighbors)) return false;
            foreach (T neighbor in neighbors) adjacency[neighbor].Remove(vertex);
            adjacency.Remove(vertex);
            return true;
        }

        /// <summary>Adds an undirected edge between two existing, distinct vertices.</summary>
        public bool AddEdge(T first, T second)
        {
            if (EqualityComparer<T>.Default.Equals(first, second)) return false;
            HashSet<T> firstNeighbors;
            HashSet<T> secondNeighbors;
            if (!adjacency.TryGetValue(first, out firstNeighbors) || !adjacency.TryGetValue(second, out secondNeighbors)) return false;
            if (!firstNeighbors.Add(second)) return false;
            secondNeighbors.Add(first);
            return true;
        }

        public bool RemoveEdge(T first, T second)
        {
            HashSet<T> firstNeighbors;
            HashSet<T> secondNeighbors;
            if (!adjacency.TryGetValue(first, out firstNeighbors) || !adjacency.TryGetValue(second, out secondNeighbors)) return false;
            bool removed = firstNeighbors.Remove(second);
            secondNeighbors.Remove(first);
            return removed;
        }

        public bool ContainsVertex(T vertex) { return adjacency.ContainsKey(vertex); }
        public bool TryFindVertex(T vertex, out T found)
        {
            HashSet<T> neighbors;
            if (adjacency.TryGetValue(vertex, out neighbors)) { found = vertex; return true; }
            found = default(T);
            return false;
        }

        public IList<T> GetNeighbors(T vertex)
        {
            HashSet<T> neighbors;
            return adjacency.TryGetValue(vertex, out neighbors) ? neighbors.OrderBy(item => item, comparer).ToList().AsReadOnly() : new List<T>().AsReadOnly();
        }

        public IDictionary<T, IList<T>> GetAdjacencyList()
        {
            Dictionary<T, IList<T>> result = new Dictionary<T, IList<T>>(adjacency.Comparer);
            foreach (KeyValuePair<T, HashSet<T>> pair in adjacency)
                result.Add(pair.Key, pair.Value.OrderBy(item => item, comparer).ToList().AsReadOnly());
            return result;
        }

        public IList<T> BreadthFirstSearch(T start) { return MyMzansi.DataStructures.BreadthFirstSearch.Traverse(this, start); }
        public IList<T> DepthFirstSearch(T start) { return MyMzansi.DataStructures.DepthFirstSearch.Traverse(this, start); }

        internal IEnumerable<T> Neighbors(T vertex) { return GetNeighbors(vertex); }
    }

    public static class BreadthFirstSearch
    {
        /// <summary>Visits reachable graph vertices level by level from the start vertex.</summary>
        public static IList<T> Traverse<T>(Graph<T> graph, T start)
        {
            if (graph == null) throw new ArgumentNullException("graph");
            List<T> visitedOrder = new List<T>();
            if (!graph.ContainsVertex(start)) return visitedOrder.AsReadOnly();
            HashSet<T> visited = new HashSet<T>();
            Queue<T> queue = new Queue<T>();
            visited.Add(start); queue.Enqueue(start);
            while (queue.Count > 0)
            {
                T current = queue.Dequeue(); visitedOrder.Add(current);
                foreach (T neighbor in graph.Neighbors(current))
                    if (visited.Add(neighbor)) queue.Enqueue(neighbor);
            }
            return visitedOrder.AsReadOnly();
        }
    }

    public static class DepthFirstSearch
    {
        /// <summary>Visits reachable graph vertices using depth-first traversal.</summary>
        public static IList<T> Traverse<T>(Graph<T> graph, T start)
        {
            if (graph == null) throw new ArgumentNullException("graph");
            List<T> visitedOrder = new List<T>();
            if (!graph.ContainsVertex(start)) return visitedOrder.AsReadOnly();
            HashSet<T> visited = new HashSet<T>();
            Stack<T> stack = new Stack<T>();
            stack.Push(start);
            while (stack.Count > 0)
            {
                T current = stack.Pop();
                if (!visited.Add(current)) continue;
                visitedOrder.Add(current);
                List<T> neighbors = graph.Neighbors(current).ToList();
                for (int i = neighbors.Count - 1; i >= 0; i--)
                    if (!visited.Contains(neighbors[i])) stack.Push(neighbors[i]);
            }
            return visitedOrder.AsReadOnly();
        }
    }
}
