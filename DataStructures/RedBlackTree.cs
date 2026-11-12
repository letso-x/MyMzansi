using System;
using System.Collections.Generic;

namespace MyMzansi.DataStructures
{
    public enum RedBlackColor { Red, Black }

    public sealed class RedBlackTree<TKey, TValue>
    {
        private sealed class Node
        {
            public TKey Key;
            public TValue Value;
            public RedBlackColor Color;
            public Node Left;
            public Node Right;
            public Node Parent;
            public Node(TKey key, TValue value, RedBlackColor color) { Key = key; Value = value; Color = color; }
        }

        private readonly IComparer<TKey> comparer;
        private readonly Node nil;
        private Node root;
        public int Count { get; private set; }

        public RedBlackTree(IComparer<TKey> keyComparer)
        {
            comparer = keyComparer ?? Comparer<TKey>.Default;
            nil = new Node(default(TKey), default(TValue), RedBlackColor.Black);
            nil.Left = nil.Right = nil.Parent = nil;
            root = nil;
        }
        public RedBlackTree() : this(null) { }

        /// <summary>Inserts a unique key and restores the red-black properties.</summary>
        public bool Insert(TKey key, TValue value)
        {
            Node parent = nil;
            Node current = root;
            while (current != nil)
            {
                parent = current;
                int comparison = comparer.Compare(key, current.Key);
                if (comparison == 0) return false;
                current = comparison < 0 ? current.Left : current.Right;
            }
            Node node = new Node(key, value, RedBlackColor.Red) { Left = nil, Right = nil, Parent = parent };
            if (parent == nil) root = node;
            else if (comparer.Compare(key, parent.Key) < 0) parent.Left = node;
            else parent.Right = node;
            InsertFixup(node);
            Count++;
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Node node = FindNode(key);
            if (node == nil) { value = default(TValue); return false; }
            value = node.Value;
            return true;
        }

        /// <summary>Removes a key and repairs the black-height invariant.</summary>
        public bool Remove(TKey key)
        {
            Node removedNode = FindNode(key);
            if (removedNode == nil) return false;
            Node replacement = removedNode;
            RedBlackColor originalColor = replacement.Color;
            Node fixNode;
            if (removedNode.Left == nil)
            {
                fixNode = removedNode.Right;
                Transplant(removedNode, removedNode.Right);
            }
            else if (removedNode.Right == nil)
            {
                fixNode = removedNode.Left;
                Transplant(removedNode, removedNode.Left);
            }
            else
            {
                replacement = Minimum(removedNode.Right);
                originalColor = replacement.Color;
                fixNode = replacement.Right;
                if (replacement.Parent == removedNode) fixNode.Parent = replacement;
                else
                {
                    Transplant(replacement, replacement.Right);
                    replacement.Right = removedNode.Right;
                    replacement.Right.Parent = replacement;
                }
                Transplant(removedNode, replacement);
                replacement.Left = removedNode.Left;
                replacement.Left.Parent = replacement;
                replacement.Color = removedNode.Color;
            }
            if (originalColor == RedBlackColor.Black) DeleteFixup(fixNode);
            Count--;
            if (root != nil) root.Parent = nil;
            nil.Parent = nil;
            return true;
        }

        /// <summary>Checks root, red-child, ordering, and equal black-height invariants.</summary>
        public bool IsValid()
        {
            if (nil.Color != RedBlackColor.Black) return false;
            if (root == nil) return Count == 0;
            if (root.Color != RedBlackColor.Black || root.Parent != nil) return false;
            int nodeCount = 0;
            int blackHeight;
            bool valid = Validate(root, false, default(TKey), false, default(TKey), out blackHeight, ref nodeCount);
            return valid && nodeCount == Count;
        }

        public IList<KeyValuePair<TKey, TValue>> InOrderTraversal()
        {
            List<KeyValuePair<TKey, TValue>> values = new List<KeyValuePair<TKey, TValue>>();
            Traverse(root, values);
            return values.AsReadOnly();
        }

        private Node FindNode(TKey key)
        {
            Node node = root;
            while (node != nil)
            {
                int comparison = comparer.Compare(key, node.Key);
                if (comparison == 0) return node;
                node = comparison < 0 ? node.Left : node.Right;
            }
            return nil;
        }

        private void InsertFixup(Node node)
        {
            while (node.Parent.Color == RedBlackColor.Red)
            {
                if (node.Parent == node.Parent.Parent.Left)
                {
                    Node uncle = node.Parent.Parent.Right;
                    if (uncle.Color == RedBlackColor.Red)
                    {
                        node.Parent.Color = RedBlackColor.Black; uncle.Color = RedBlackColor.Black;
                        node.Parent.Parent.Color = RedBlackColor.Red; node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Right) { node = node.Parent; RotateLeft(node); }
                        node.Parent.Color = RedBlackColor.Black; node.Parent.Parent.Color = RedBlackColor.Red; RotateRight(node.Parent.Parent);
                    }
                }
                else
                {
                    Node uncle = node.Parent.Parent.Left;
                    if (uncle.Color == RedBlackColor.Red)
                    {
                        node.Parent.Color = RedBlackColor.Black; uncle.Color = RedBlackColor.Black;
                        node.Parent.Parent.Color = RedBlackColor.Red; node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Left) { node = node.Parent; RotateRight(node); }
                        node.Parent.Color = RedBlackColor.Black; node.Parent.Parent.Color = RedBlackColor.Red; RotateLeft(node.Parent.Parent);
                    }
                }
            }
            root.Color = RedBlackColor.Black;
            root.Parent = nil;
        }

        private void DeleteFixup(Node node)
        {
            while (node != root && node.Color == RedBlackColor.Black)
            {
                if (node == node.Parent.Left)
                {
                    Node sibling = node.Parent.Right;
                    if (sibling.Color == RedBlackColor.Red)
                    {
                        sibling.Color = RedBlackColor.Black; node.Parent.Color = RedBlackColor.Red; RotateLeft(node.Parent); sibling = node.Parent.Right;
                    }
                    if (sibling.Left.Color == RedBlackColor.Black && sibling.Right.Color == RedBlackColor.Black)
                    {
                        if (sibling != nil) sibling.Color = RedBlackColor.Red;
                        node = node.Parent;
                    }
                    else
                    {
                        if (sibling.Right.Color == RedBlackColor.Black)
                        {
                            sibling.Left.Color = RedBlackColor.Black; sibling.Color = RedBlackColor.Red; RotateRight(sibling); sibling = node.Parent.Right;
                        }
                        sibling.Color = node.Parent.Color; node.Parent.Color = RedBlackColor.Black; sibling.Right.Color = RedBlackColor.Black; RotateLeft(node.Parent); node = root;
                    }
                }
                else
                {
                    Node sibling = node.Parent.Left;
                    if (sibling.Color == RedBlackColor.Red)
                    {
                        sibling.Color = RedBlackColor.Black; node.Parent.Color = RedBlackColor.Red; RotateRight(node.Parent); sibling = node.Parent.Left;
                    }
                    if (sibling.Right.Color == RedBlackColor.Black && sibling.Left.Color == RedBlackColor.Black)
                    {
                        if (sibling != nil) sibling.Color = RedBlackColor.Red;
                        node = node.Parent;
                    }
                    else
                    {
                        if (sibling.Left.Color == RedBlackColor.Black)
                        {
                            sibling.Right.Color = RedBlackColor.Black; sibling.Color = RedBlackColor.Red; RotateLeft(sibling); sibling = node.Parent.Left;
                        }
                        sibling.Color = node.Parent.Color; node.Parent.Color = RedBlackColor.Black; sibling.Left.Color = RedBlackColor.Black; RotateRight(node.Parent); node = root;
                    }
                }
            }
            node.Color = RedBlackColor.Black;
        }

        private void RotateLeft(Node pivot)
        {
            Node promoted = pivot.Right;
            pivot.Right = promoted.Left;
            if (promoted.Left != nil) promoted.Left.Parent = pivot;
            promoted.Parent = pivot.Parent;
            if (pivot.Parent == nil) root = promoted;
            else if (pivot == pivot.Parent.Left) pivot.Parent.Left = promoted;
            else pivot.Parent.Right = promoted;
            promoted.Left = pivot;
            pivot.Parent = promoted;
        }

        private void RotateRight(Node pivot)
        {
            Node promoted = pivot.Left;
            pivot.Left = promoted.Right;
            if (promoted.Right != nil) promoted.Right.Parent = pivot;
            promoted.Parent = pivot.Parent;
            if (pivot.Parent == nil) root = promoted;
            else if (pivot == pivot.Parent.Right) pivot.Parent.Right = promoted;
            else pivot.Parent.Left = promoted;
            promoted.Right = pivot;
            pivot.Parent = promoted;
        }

        private void Transplant(Node oldNode, Node newNode)
        {
            if (oldNode.Parent == nil) root = newNode;
            else if (oldNode == oldNode.Parent.Left) oldNode.Parent.Left = newNode;
            else oldNode.Parent.Right = newNode;
            newNode.Parent = oldNode.Parent;
        }

        private Node Minimum(Node node) { while (node.Left != nil) node = node.Left; return node; }

        private bool Validate(Node node, bool hasLower, TKey lower, bool hasUpper, TKey upper, out int blackHeight, ref int count)
        {
            if (node == nil) { blackHeight = 1; return true; }
            count++;
            if ((hasLower && comparer.Compare(node.Key, lower) <= 0) || (hasUpper && comparer.Compare(node.Key, upper) >= 0)) { blackHeight = 0; return false; }
            if (node.Left == null || node.Right == null || node.Parent == null) { blackHeight = 0; return false; }
            if (node.Color == RedBlackColor.Red && (node.Left.Color == RedBlackColor.Red || node.Right.Color == RedBlackColor.Red)) { blackHeight = 0; return false; }
            int leftHeight, rightHeight;
            bool leftValid = Validate(node.Left, hasLower, lower, true, node.Key, out leftHeight, ref count);
            bool rightValid = Validate(node.Right, true, node.Key, hasUpper, upper, out rightHeight, ref count);
            blackHeight = Math.Max(leftHeight, rightHeight) + (node.Color == RedBlackColor.Black ? 1 : 0);
            return leftValid && rightValid && leftHeight == rightHeight;
        }

        private void Traverse(Node node, IList<KeyValuePair<TKey, TValue>> result)
        {
            if (node == nil) return;
            Traverse(node.Left, result); result.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value)); Traverse(node.Right, result);
        }
    }
}
