using System;
using System.Collections.Generic;
using System.Linq;

namespace MyMzansi.DataStructures
{
    public sealed class MinimumSpanningTreeResult<T>
    {
        public IList<WeightedEdge<T>> Edges { get; private set; }
        public int TotalWeight { get; private set; }
        public bool IsSpanningTree { get; private set; }
        public MinimumSpanningTreeResult(IList<WeightedEdge<T>> edges, int totalWeight, bool isSpanningTree)
        {
            Edges = new List<WeightedEdge<T>>(edges).AsReadOnly(); TotalWeight = totalWeight; IsSpanningTree = isSpanningTree;
        }
    }

    public static class KruskalMst
    {
        /// <summary>Builds a minimum spanning tree (or forest for disconnected input) using Union-Find.</summary>
        public static MinimumSpanningTreeResult<T> Find<T>(WeightedGraph<T> graph, IComparer<T> tieBreaker)
        {
            if (graph == null) throw new ArgumentNullException("graph");
            IComparer<T> comparer = tieBreaker ?? Comparer<T>.Default;
            List<WeightedEdge<T>> sorted = graph.Edges.OrderBy(edge => edge.Weight)
                .ThenBy(edge => edge.First, comparer).ThenBy(edge => edge.Second, comparer).ToList();
            DisjointSet<T> components = new DisjointSet<T>(graph.Vertices);
            List<WeightedEdge<T>> selected = new List<WeightedEdge<T>>();
            int totalWeight = 0;
            foreach (WeightedEdge<T> edge in sorted)
            {
                if (!components.Union(edge.First, edge.Second)) continue;
                selected.Add(edge);
                totalWeight += edge.Weight;
                if (selected.Count == graph.VertexCount - 1) break;
            }
            bool isTree = graph.VertexCount == 0 || graph.VertexCount == 1 || selected.Count == graph.VertexCount - 1;
            return new MinimumSpanningTreeResult<T>(selected, totalWeight, isTree);
        }

        public static MinimumSpanningTreeResult<T> Find<T>(WeightedGraph<T> graph) { return Find(graph, null); }
    }
}
