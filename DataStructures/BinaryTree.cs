using System;
using System.Collections.Generic;

namespace MyMzansi.DataStructures
{
    public sealed class BinaryTreeNode<T>
    {
        public T Value { get; internal set; }
        public BinaryTreeNode<T> Left { get; internal set; }
        public BinaryTreeNode<T> Right { get; internal set; }

        internal BinaryTreeNode(T value) { Value = value; }
    }

    public sealed class BinaryTree<T>
    {
        private readonly Queue<BinaryTreeNode<T>> insertionQueue = new Queue<BinaryTreeNode<T>>();
        private BinaryTreeNode<T> root;
        public BinaryTreeNode<T> Root { get { return root; } }
        public int Count { get; private set; }

        /// <summary>Inserts values level by level to keep this a complete binary tree.</summary>
        public void Insert(T value)
        {
            BinaryTreeNode<T> node = new BinaryTreeNode<T>(value);
            if (root == null)
            {
                root = node;
                insertionQueue.Enqueue(node);
                Count++;
                return;
            }

            while (insertionQueue.Count > 0)
            {
                BinaryTreeNode<T> parent = insertionQueue.Peek();
                if (parent.Left == null) { parent.Left = node; break; }
                if (parent.Right == null) { parent.Right = node; insertionQueue.Dequeue(); break; }
                insertionQueue.Dequeue();
            }
            insertionQueue.Enqueue(node);
            Count++;
        }

        /// <summary>Searches the tree breadth-first using a caller-provided match condition.</summary>
        public T Find(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException("match");
            if (root == null) return default(T);
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                BinaryTreeNode<T> node = queue.Dequeue();
                if (match(node.Value)) return node.Value;
                if (node.Left != null) queue.Enqueue(node.Left);
                if (node.Right != null) queue.Enqueue(node.Right);
            }
            return default(T);
        }

        public IList<T> PreOrderTraversal() { List<T> result = new List<T>(); PreOrder(root, result); return result.AsReadOnly(); }
        public IList<T> InOrderTraversal() { List<T> result = new List<T>(); InOrder(root, result); return result.AsReadOnly(); }
        public IList<T> PostOrderTraversal() { List<T> result = new List<T>(); PostOrder(root, result); return result.AsReadOnly(); }

        private static void PreOrder(BinaryTreeNode<T> node, IList<T> result)
        {
            if (node == null) return;
            result.Add(node.Value); PreOrder(node.Left, result); PreOrder(node.Right, result);
        }
        private static void InOrder(BinaryTreeNode<T> node, IList<T> result)
        {
            if (node == null) return;
            InOrder(node.Left, result); result.Add(node.Value); InOrder(node.Right, result);
        }
        private static void PostOrder(BinaryTreeNode<T> node, IList<T> result)
        {
            if (node == null) return;
            PostOrder(node.Left, result); PostOrder(node.Right, result); result.Add(node.Value);
        }
    }
}
