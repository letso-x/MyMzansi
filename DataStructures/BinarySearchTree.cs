using System;
using System.Collections.Generic;

namespace MyMzansi.DataStructures
{
    public sealed class BinarySearchTree<TKey, TValue>
    {
        private sealed class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left;
            public Node Right;
            public Node(TKey key, TValue value) { Key = key; Value = value; }
        }

        private readonly IComparer<TKey> comparer;
        private Node root;
        public int Count { get; private set; }

        public BinarySearchTree(IComparer<TKey> keyComparer) { comparer = keyComparer ?? Comparer<TKey>.Default; }
        public BinarySearchTree() : this(null) { }

        /// <summary>Inserts a key/value pair. Returns false when the key already exists.</summary>
        public bool Insert(TKey key, TValue value)
        {
            bool inserted = false;
            root = Insert(root, key, value, ref inserted);
            if (inserted) Count++;
            return inserted;
        }

        private Node Insert(Node node, TKey key, TValue value, ref bool inserted)
        {
            if (node == null) { inserted = true; return new Node(key, value); }
            int comparison = comparer.Compare(key, node.Key);
            if (comparison < 0) node.Left = Insert(node.Left, key, value, ref inserted);
            else if (comparison > 0) node.Right = Insert(node.Right, key, value, ref inserted);
            return node;
        }

        /// <summary>Looks up a key in O(h) time, where h is the tree height.</summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Node node = root;
            while (node != null)
            {
                int comparison = comparer.Compare(key, node.Key);
                if (comparison == 0) { value = node.Value; return true; }
                node = comparison < 0 ? node.Left : node.Right;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>Removes a key and rejoins its subtrees.</summary>
        public bool Remove(TKey key)
        {
            bool removed = false;
            root = Remove(root, key, ref removed);
            if (removed) Count--;
            return removed;
        }

        private Node Remove(Node node, TKey key, ref bool removed)
        {
            if (node == null) return null;
            int comparison = comparer.Compare(key, node.Key);
            if (comparison < 0) node.Left = Remove(node.Left, key, ref removed);
            else if (comparison > 0) node.Right = Remove(node.Right, key, ref removed);
            else
            {
                removed = true;
                if (node.Left == null) return node.Right;
                if (node.Right == null) return node.Left;
                Node successor = Minimum(node.Right);
                node.Key = successor.Key; node.Value = successor.Value;
                bool successorRemoved = false;
                node.Right = Remove(node.Right, successor.Key, ref successorRemoved);
            }
            return node;
        }

        public IList<KeyValuePair<TKey, TValue>> InOrderTraversal() { List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>(); Traverse(root, result, 0); return result.AsReadOnly(); }
        public IList<KeyValuePair<TKey, TValue>> PreOrderTraversal() { List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>(); Traverse(root, result, 1); return result.AsReadOnly(); }
        public IList<KeyValuePair<TKey, TValue>> PostOrderTraversal() { List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>(); Traverse(root, result, 2); return result.AsReadOnly(); }

        private static Node Minimum(Node node) { while (node.Left != null) node = node.Left; return node; }
        private static void Traverse(Node node, IList<KeyValuePair<TKey, TValue>> result, int order)
        {
            if (node == null) return;
            if (order == 1) result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
            Traverse(node.Left, result, order);
            if (order == 0) result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
            Traverse(node.Right, result, order);
            if (order == 2) result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
        }
    }
}
