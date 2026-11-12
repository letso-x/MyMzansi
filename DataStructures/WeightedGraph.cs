using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMzansi.DataStructures
{
    public sealed class WeightedEdge<T>
    {
        public T First { get; private set; }
        public T Second { get; private set; }
        public int Weight { get; private set; }
        public WeightedEdge(T first, T second, int weight) { First = first; Second = second; Weight = weight; }
        public override string ToString() { return Convert.ToString(First) + " — " + Convert.ToString(Second) + " (" + Weight + ")"; }
    }

    public sealed class WeightedGraph<T>
    {
        private readonly HashSet<T> vertices;
        private readonly List<WeightedEdge<T>> edges = new List<WeightedEdge<T>>();
        public IEnumerable<T> Vertices { get { return vertices.ToList().AsReadOnly(); } }
        public IList<WeightedEdge<T>> Edges { get { return edges.AsReadOnly(); } }
        public int VertexCount { get { return vertices.Count; } }

        public WeightedGraph(IEqualityComparer<T> equalityComparer) { vertices = new HashSet<T>(equalityComparer ?? EqualityComparer<T>.Default); }
        public WeightedGraph() : this(null) { }
        public bool AddVertex(T vertex) { return vertices.Add(vertex); }

        /// <summary>Adds an illustrative, undirected weighted edge between existing vertices.</summary>
        public bool AddEdge(T first, T second, int weight)
        {
            if (!vertices.Contains(first) || !vertices.Contains(second) || EqualityComparer<T>.Default.Equals(first, second)) return false;
            if (edges.Any(edge => (vertices.Comparer.Equals(edge.First, first) && vertices.Comparer.Equals(edge.Second, second)) ||
                                  (vertices.Comparer.Equals(edge.First, second) && vertices.Comparer.Equals(edge.Second, first)))) return false;
            edges.Add(new WeightedEdge<T>(first, second, weight));
            return true;
        }
    }
}
