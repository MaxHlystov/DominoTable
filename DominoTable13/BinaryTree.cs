using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DominoTable13
{

public class BinaryTree<T> : ICollection<T> where T : IComparable<T>
{
    protected class Node
    {
        public T Value
        {
            get;
            set;
        }
        public Node Left
        {
            get;
            set;
        }
        public Node Right
        {
            get;
            set;
        }

        public Node(T value)
        {
            Value = value;
        }
    }

    protected Node root;
    protected IComparer<T> comparer;

    public BinaryTree()
        : this(Comparer<T>.Default)
    {
    }
    public BinaryTree(IComparer<T> defaultComparer)
    {
        if (defaultComparer == null)
            throw new ArgumentNullException("Default comparer is null");
        comparer = defaultComparer;
    }
    public BinaryTree(IEnumerable<T> collection)
        : this(collection, Comparer<T>.Default)
    {

    }
    public BinaryTree(IEnumerable<T> collection, IComparer<T> defaultComparer)
        : this(defaultComparer)
    {
        AddRange(collection);
    }

    public T MinValue
    {
        get
        {
            if (root == null)
                throw new InvalidOperationException("Tree is empty");
            var current = root;
            while (current.Left != null)
                current = current.Left;
            return current.Value;
        }
    }
    public T MaxValue
    {
        get
        {
            if (root == null)
                throw new InvalidOperationException("Tree is empty");
            var current = root;
            while (current.Right != null)
                current = current.Right;
            return current.Value;
        }
    }

    public void AddRange(IEnumerable<T> collection)
    {
        foreach (var value in collection)
            Add(value);
    }

    public IEnumerable<T> Inorder()
    {
        if (root == null)
            yield break;

        var stack = new Stack<Node>();
        var node = root;

        while (stack.Count > 0 || node != null)
        {
            if (node == null)
            {
                node = stack.Pop();
                yield return node.Value;
                node = node.Right;
            }
            else
            {
                stack.Push(node);
                node = node.Left;
            }
        }
    }
    public IEnumerable<T> Preorder()
    {
        if (root == null)
            yield break;

        var stack = new Stack<Node>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return node.Value;
            if (node.Right != null)
                stack.Push(node.Right);
            if (node.Left != null)
                stack.Push(node.Left);
        }
    }
    public IEnumerable<T> Postorder()
    {
        if (root == null)
            yield break;

        var stack = new Stack<Node>();
        var node = root;

        while (stack.Count > 0 || node != null)
        {
            if (node == null)
            {
                node = stack.Pop();
                if (stack.Count > 0 && node.Right == stack.Peek())
                {
                    stack.Pop();
                    stack.Push(node);
                    node = node.Right;
                }
                else
                {
                    yield return node.Value;
                    node = null;
                }
            }
            else
            {
                if (node.Right != null)
                    stack.Push(node.Right);
                stack.Push(node);
                node = node.Left;
            }
        }
    }
    public IEnumerable<T> Levelorder()
    {
        if (root == null)
            yield break;

        var queue = new Queue<Node>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node.Value;
            if (node.Left != null)
                queue.Enqueue(node.Left);
            if (node.Right != null)
                queue.Enqueue(node.Right);
        }
    }

    #region ICollection<T> Members
    public int Count
    {
        get;
        protected set;
    }
    public virtual void Add(T item)
    {
        var node = new Node(item);

        if (root == null)
            root = node;
        else
        {
            Node current = root, parent = null;

            while (current != null)
            {
                parent = current;
                if (comparer.Compare(item, current.Value) < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }

            if (comparer.Compare(item, parent.Value) < 0)
                parent.Left = node;
            else
                parent.Right = node;
        }
        ++Count;
    }
    public virtual bool Remove(T item)
    {
        if (root == null)
            return false;

        Node current = root, parent = null;

        int result;
        do
        {
            result = comparer.Compare(item, current.Value);
            if (result < 0)
            {
                parent = current;
                current = current.Left;
            }
            else if (result > 0)
            {
                parent = current;
                current = current.Right;
            }
            if (current == null)
                return false;
        }
        while (result != 0);

        if (current.Right == null)
        {
            if (current == root)
                root = current.Left;
            else
            {
                result = comparer.Compare(current.Value, parent.Value);
                if (result < 0)
                    parent.Left = current.Left;
                else
                    parent.Right = current.Left;
            }
        }
        else if (current.Right.Left == null)
        {
            current.Right.Left = current.Left;
            if (current == root)
                root = current.Right;
            else
            {
                result = comparer.Compare(current.Value, parent.Value);
                if (result < 0)
                    parent.Left = current.Right;
                else
                    parent.Right = current.Right;
            }
        }
        else
        {
            Node min = current.Right.Left, prev = current.Right;
            while (min.Left != null)
            {
                prev = min;
                min = min.Left;
            }
            prev.Left = min.Right;
            min.Left = current.Left;
            min.Right = current.Right;

            if (current == root)
                root = min;
            else
            {
                result = comparer.Compare(current.Value, parent.Value);
                if (result < 0)
                    parent.Left = min;
                else
                    parent.Right = min;
            }
        }
        --Count;
        return true;
    }
    public void Clear()
    {
        root = null;
        Count = 0;
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var value in this)
            array[arrayIndex++] = value;
    }
    public virtual bool IsReadOnly
    {
        get
        {
            return false;
        }
    }
    public bool Contains(T item)
    {
        var current = root;
        while (current != null)
        {
            var result = comparer.Compare(item, current.Value);
            if (result == 0)
                return true;
            if (result < 0)
                current = current.Left;
            else
                current = current.Right;
        }
        return false;
    }
    #endregion

    #region IEnumerable<T> Members
    public IEnumerator<T> GetEnumerator()
    {
        return Inorder().GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
}


}
