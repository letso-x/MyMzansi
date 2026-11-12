using System;
using System.Collections.Generic;

namespace MyMzansi.DataStructures
{
    public sealed class AVLTree<TKey, TValue>
    {
        private sealed class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left;
            public Node Right;
            public int Height;
            public Node(TKey key, TValue value) { Key = key; Value = value; Height = 1; }
        }

        private readonly IComparer<TKey> comparer;
        private Node root;
        public int Count { get; private set; }
        public int Height { get { return HeightOf(root); } }

        public AVLTree(IComparer<TKey> keyComparer) { comparer = keyComparer ?? Comparer<TKey>.Default; }
        public AVLTree() : this(null) { }

        /// <summary>Inserts a unique key and rebalances every ancestor.</summary>
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
            else return node;
            return Balance(Update(node));
        }

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

        /// <summary>Deletes a key and restores the AVL height invariant.</summary>
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
            return Balance(Update(node));
        }

        public int GetBalanceFactor(TKey key)
        {
            Node node = root;
            while (node != null)
            {
                int comparison = comparer.Compare(key, node.Key);
                if (comparison == 0) return HeightOf(node.Left) - HeightOf(node.Right);
                node = comparison < 0 ? node.Left : node.Right;
            }
            return 0;
        }

        public IList<KeyValuePair<TKey, TValue>> InOrderTraversal() { List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>(); Traverse(root, result); return result.AsReadOnly(); }

        private Node Balance(Node node)
        {
            int factor = HeightOf(node.Left) - HeightOf(node.Right);
            if (factor > 1)
            {
                if (HeightOf(node.Left.Left) < HeightOf(node.Left.Right)) node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }
            if (factor < -1)
            {
                if (HeightOf(node.Right.Right) < HeightOf(node.Right.Left)) node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }
            return node;
        }

        private static Node RotateRight(Node pivot)
        {
            Node promoted = pivot.Left;
            pivot.Left = promoted.Right;
            promoted.Right = pivot;
            Update(pivot); Update(promoted);
            return promoted;
        }

        private static Node RotateLeft(Node pivot)
        {
            Node promoted = pivot.Right;
            pivot.Right = promoted.Left;
            promoted.Left = pivot;
            Update(pivot); Update(promoted);
            return promoted;
        }

        private static Node Update(Node node)
        {
            if (node != null) node.Height = Math.Max(HeightOf(node.Left), HeightOf(node.Right)) + 1;
            return node;
        }
        private static int HeightOf(Node node) { return node == null ? 0 : node.Height; }
        private static Node Minimum(Node node) { while (node.Left != null) node = node.Left; return node; }
        private static void Traverse(Node node, IList<KeyValuePair<TKey, TValue>> result)
        {
            if (node == null) return;
            Traverse(node.Left, result); result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value)); Traverse(node.Right, result);
        }
    }
}
