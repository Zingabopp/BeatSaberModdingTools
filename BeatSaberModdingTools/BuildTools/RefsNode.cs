using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BeatSaberModdingTools.BuildTools
{
    public enum RefsNodesType
    {
        Unknown = 0,
        Root = 1,
        Command = 2,
        Leaf = 3,
        File = 4
    }
    public abstract class RefsNode : IList<RefsNode>
    {
        public abstract RefsNodesType NodeType { get; }
        public abstract string RawLine { get; }
        public virtual RefsNode Parent { get; protected set; }
        public abstract int NodeDepth { get; }
        public abstract bool SupportsChildren { get; }
        public int Count => Children?.Count ?? 0;

        /// <summary>
        /// Gets the child <see cref="RefsNode"/> at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if the node doesn't support children.</exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public RefsNode this[int index]
        {
            get
            {
                if (!SupportsChildren)
                    throw new InvalidOperationException($"RefsNode type {GetType().Name} does not support children.");
                if (index > Children.Count - 1)
                    throw new IndexOutOfRangeException($"Index of {index} is greater than the last child index of {Children.Count - 1}");
                return Children[index];
            }
            set
            {
                if (!SupportsChildren)
                    throw new InvalidOperationException($"RefsNode type {GetType().Name} does not support children.");
                if (index > Children.Count - 1)
                    throw new IndexOutOfRangeException($"Index of {index} is greater than the last child index of {Children.Count - 1}");
                RefsNode previous = Children[index];
                Children[index] = value;
                if (previous != value)
                {
                    if (previous.Parent == this)
                        previous.SetParent(null);
                    value.SetParent(this);
                }
            }
        }
        protected virtual void ClearCachedData()
        {
            return;
            if (SupportsChildren)
                Children.ForEach(c => c.ClearCachedData());
        }
        public abstract bool TryGetReference(string fileName, out FileNode fileNode);

        public virtual RefsNode[] GetChildren()
        {
            return (!SupportsChildren || Children.Count == 0) ? Array.Empty<RefsNode>() : Children.ToArray();
        }
        protected RefsNode()
        {
            Children = SupportsChildren ? new List<RefsNode>() : null;
        }
        protected List<RefsNode> Children { get; }

        public bool IsReadOnly => SupportsChildren;

        public string[] GetLines()
        {
            List<string> lines = new List<string>();
            GetLines(ref lines);
            return lines.ToArray();
        }

        protected virtual void GetLines(ref List<string> list)
        {
            list.Add(RawLine);
            if (SupportsChildren)
            {
                foreach (RefsNode childNode in GetChildren())
                {
                    childNode.GetLines(ref list);
                }
            }
        }

        protected virtual void SetParent(RefsNode newParent)
        {
            if (Parent == newParent)
                return;
            if (Parent != null)
                Parent.Children.Remove(this);
            Parent = newParent;
            if (Parent != null && !Parent.Contains(this))
                Parent.Children.Add(this);
            ClearCachedData();
        }

        public override string ToString()
        {
            return RawLine;
        }

        public int IndexOf(RefsNode item)
        {
            return Children.IndexOf(item);
        }

        /// <summary>
        /// Inserts a child node at the provided index. If <paramref name="index"/> is greater than the number of children, <paramref name="child"/> will be added to the end.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="child"/> is null.</exception>
        /// <exception cref="NotSupportedException">Thrown when this <see cref="RefsNode"/> doesn't support children.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is less than 0.</exception>
        public virtual void Insert(int index, RefsNode child)
        {
            if (child == null)
                throw new ArgumentNullException($"Cannot insert a null child node.");
            if (!SupportsChildren)
                throw new NotSupportedException($"RefsNode type {GetType().Name} does not support children");
            if (index < 0)
                throw new ArgumentOutOfRangeException($"{index} cannot be negative for {nameof(Insert)}");
            if (index > Children.Count)
                index = Children.Count;
            Children.Insert(index, child);
            child.SetParent(this);
        }

        /// <summary>
        /// Removes and returns the child at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual RefsNode RemoveChildAt(int index)
        {
            RefsNode toRemove = Children[index];
            Children.RemoveAt(index);
            if (toRemove.Parent == this)
                toRemove.SetParent(null);
            return toRemove;
        }

        /// <summary>
        /// Removes the child at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual void RemoveAt(int index)
        {
            RemoveChildAt(index);
        }

        /// <summary>
        /// Adds the provided <see cref="RefsNode"/> as a child. Sets <paramref name="child"/>'s <see cref="Parent"/> to this node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns>The child node that was added.</returns>
        /// <exception cref="NotSupportedException">Thrown when children aren't supported on this <see cref="RefsNode"/></exception>
        public virtual void Add(RefsNode child)
        {
            if (!SupportsChildren)
                throw new NotSupportedException($"RefsNode type {GetType().Name} does not support children");
            if (child == this)
                throw new ArgumentException($"A parent node cannot also be its child.");
            Children.Add(child);
            child.Parent = this;
            child.ClearCachedData();
        }

        public virtual void Clear()
        {
            foreach (var child in Children)
            {
                if (child.Parent == this)
                    child.SetParent(null);
            }
            Children.Clear();
        }

        public virtual bool Contains(RefsNode item)
        {
            return Children.Contains(item);
        }

        public virtual void CopyTo(RefsNode[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(RefsNode child)
        {
            if (Children.Remove(child))
            {
                if (child.Parent == this)
                    child.SetParent(null);
                return true;
            }
            return false;
        }

        public virtual IEnumerator<RefsNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }

}
