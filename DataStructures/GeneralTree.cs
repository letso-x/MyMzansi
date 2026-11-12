using System;
using System.Collections.Generic;
using System.Text;

namespace MyMzansi.DataStructures
{
    public sealed class GeneralTreeNode<T>
    {
        private readonly List<GeneralTreeNode<T>> children = new List<GeneralTreeNode<T>>();

        public T Value { get; private set; }
        public GeneralTreeNode<T> Parent { get; private set; }
        public IList<GeneralTreeNode<T>> Children { get { return children.AsReadOnly(); } }

        internal GeneralTreeNode(T value, GeneralTreeNode<T> parent)
        {
            Value = value;
            Parent = parent;
        }

        internal GeneralTreeNode<T> Add(T value)
        {
            GeneralTreeNode<T> node = new GeneralTreeNode<T>(value, this);
            children.Add(node);
            return node;
        }

        internal bool Remove(GeneralTreeNode<T> node) { return children.Remove(node); }
    }

    public sealed class GeneralTree<T>
    {
        private readonly IEqualityComparer<T> comparer;
        public GeneralTreeNode<T> Root { get; private set; }
        public bool IsEmpty { get { return Root == null; } }

        public GeneralTree(IEqualityComparer<T> equalityComparer)
        {
            comparer = equalityComparer ?? EqualityComparer<T>.Default;
        }

        public GeneralTree() : this(null) { }

        /// <summary>Adds the root or attaches a unique value below an existing parent.</summary>
        public bool Add(T parentValue, T value)
        {
            if (Root == null)
            {
                Root = new GeneralTreeNode<T>(value, null);
                return true;
            }
            if (Find(value) != null) return false;
            GeneralTreeNode<T> parent = Find(parentValue);
            if (parent == null) return false;
            parent.Add(value);
            return true;
        }

        /// <summary>Finds the first node with a matching value.</summary>
        public GeneralTreeNode<T> Find(T value)
        {
            if (Root == null) return null;
            Stack<GeneralTreeNode<T>> stack = new Stack<GeneralTreeNode<T>>();
            stack.Push(Root);
            while (stack.Count > 0)
            {
                GeneralTreeNode<T> node = stack.Pop();
                if (comparer.Equals(node.Value, value)) return node;
                for (int i = node.Children.Count - 1; i >= 0; i--) stack.Push(node.Children[i]);
            }
            return null;
        }

        /// <summary>Removes a node and its entire subtree; removing the root empties the tree.</summary>
        public bool Remove(T value)
        {
            GeneralTreeNode<T> node = Find(value);
            if (node == null) return false;
            if (node.Parent == null) Root = null;
            else node.Parent.Remove(node);
            return true;
        }

        /// <summary>Returns values in pre-order depth-first order.</summary>
        public IList<T> TraverseDepthFirst()
        {
            List<T> values = new List<T>();
            if (Root == null) return values.AsReadOnly();
            Stack<GeneralTreeNode<T>> stack = new Stack<GeneralTreeNode<T>>();
            stack.Push(Root);
            while (stack.Count > 0)
            {
                GeneralTreeNode<T> node = stack.Pop();
                values.Add(node.Value);
                for (int i = node.Children.Count - 1; i >= 0; i--) stack.Push(node.Children[i]);
            }
            return values.AsReadOnly();
        }

        /// <summary>Returns the hierarchy as indented lines.</summary>
        public string FormatHierarchy()
        {
            StringBuilder output = new StringBuilder();
            AppendHierarchy(Root, 0, output);
            return output.ToString();
        }

        private static void AppendHierarchy(GeneralTreeNode<T> node, int depth, StringBuilder output)
        {
            if (node == null) return;
            output.Append(' ', depth * 2).AppendLine(Convert.ToString(node.Value));
            foreach (GeneralTreeNode<T> child in node.Children) AppendHierarchy(child, depth + 1, output);
        }
    }
}
