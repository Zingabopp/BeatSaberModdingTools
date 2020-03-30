using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        #region Public Abstract
        public abstract RefsNodesType NodeType { get; }
        public abstract string RawLine { get; }
        public abstract int NodeDepth { get; }
        public abstract bool SupportsChildren { get; }

        public abstract bool TryGetReference(string fileName, out FileNode fileNode);
        #endregion

        public virtual RefsNode Parent { get; protected set; }

        public virtual RefsNode[] GetChildren()
        {
            return (!SupportsChildren || Children.Count == 0) ? Array.Empty<RefsNode>() : Children.ToArray();
        }


        public virtual T Find<T>(Func<T, bool> func) where T : RefsNode
        {
            if (this is T target && func(target))
                return target;
            foreach (var child in Children)
            {
                T obj = child.Find(func);
                if (obj != null)
                    return obj;
            }
            return null;
        }
        public virtual T[] FindAll<T>(Func<T, bool> func, int maxDepth = int.MaxValue) where T : RefsNode
        {
            List<T> matches = new List<T>();
            FindAll(func, ref matches, maxDepth);
            return matches.ToArray();
        }


        protected virtual void FindAll<T>(Func<T, bool> func, ref List<T> matches, int maxDepth) where T : RefsNode
        {
            if (maxDepth < 0)
                return;
            if (this is T target && func(target))
                matches.Add(target);
            foreach (var child in Children)
            {
                child.FindAll(func, ref matches, maxDepth - 1);
            }
        }



        private bool? _inOptionalBlock;
        private string _pathSource;

        public bool InOptionalBlock()
        {
            if (_inOptionalBlock != null)
                return _inOptionalBlock ?? false;
            if (this is CommandNode cmdNode && cmdNode.Command == CommandNode.CommandType.OptionalBlock)
                return true;
            _inOptionalBlock = false;
            if (Parent != null)
                _inOptionalBlock = Parent.InOptionalBlock();
            return _inOptionalBlock ?? false;
        }

        public string GetPathSource()
        {
            if (!string.IsNullOrEmpty(_pathSource))
                return _pathSource;
            if (this is CommandNode cmdNode)
            {
                if (cmdNode.Command == CommandNode.CommandType.From)
                {
                    _pathSource = cmdNode.CommandData;
                    return _pathSource;
                }
                else if (cmdNode.Command == CommandNode.CommandType.Prompt)
                {
                    _pathSource = CommandNode.PromptSourceTag;
                    return _pathSource;
                }
            }
            if (Parent != null)
                _pathSource = Parent.GetPathSource();
            return _pathSource;
        }

        public virtual bool InsertReference(FileEntry fileEntry, bool optional = false)
        {
            LeafNode[] matches = FindAll<LeafNode>(l =>
            {
                if (l.NodeType == RefsNodesType.File 
                || l.InOptionalBlock() != optional
                || l.GetPathSource() != fileEntry.PathSource)
                    return false;
                string relativePath = l.GetRelativePath();
                if(fileEntry.Fullname.StartsWith(relativePath))
                    return true;
                return false;
            });
            LeafNode bestMatch = matches.OrderByDescending(l => l.GetRelativePath().Length).FirstOrDefault();
            if (bestMatch != null)
            {
                for(int i = 0; i < bestMatch.Count; i++)
                {
                    if(bestMatch[i].Count > 0)
                    {
                        bestMatch.AddFile(fileEntry);
                        return true;
                    }
                }
            }
            else
            {
                RootNode rootNode = this as RootNode;
                CommandNode matchedCommandNode = this as CommandNode;
                if (rootNode != null)
                {
                    matchedCommandNode = rootNode.FindAll<CommandNode>(c => c.GetPathSource() == fileEntry.PathSource && c.InOptionalBlock() == optional, 2).FirstOrDefault();
                    if (matchedCommandNode == null)
                        return false;
                }
                string[] pathParts = fileEntry.Fullname.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if(pathParts.Length == 1)
                {
                    matchedCommandNode.Insert(0, new FileNode(fileEntry.Fullname, fileEntry));
                    return true;
                }
                RefsNode current = matchedCommandNode;
                for (int i = 0; i < pathParts.Length - 1; i++)
                {
                    LeafNode newLeaf = new LeafNode(pathParts[i] + "/");
                    current.Add(newLeaf);
                    current = newLeaf;
                }
                if (current is LeafNode leaf)
                {
                    leaf.AddFile(fileEntry);
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Removes and returns the child at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotSupportedException">Thrown when children aren't supported on this <see cref="RefsNode"/></exception>
        public virtual RefsNode RemoveChildAt(int index)
        {
            RefsNode toRemove = Children[index];
            Children.RemoveAt(index);
            if (toRemove.Parent == this)
                toRemove.SetParent(null);
            return toRemove;
        }

        public override string ToString()
        {
            return RawLine;
        }

        public string[] GetLines()
        {
            List<string> lines = new List<string>();
            GetLines(ref lines);
            return lines.ToArray();
        }

        #region Protected
        protected RefsNode()
        {
            if (SupportsChildren)
                Children = new List<RefsNode>();
            else
                Children = ReadOnlyList<RefsNode>.Empty;
        }

        protected IList<RefsNode> Children { get; }

        /// <summary>
        /// Indicates that this <see cref="RefsNode"/> does not contribute any lines to the text.
        /// </summary>
        protected virtual bool NoOutput => false;

        protected virtual void GetLines(ref List<string> list)
        {
            if (!NoOutput)
                list.Add(RawLine);
            if (SupportsChildren)
            {
                foreach (RefsNode childNode in GetChildren())
                {
                    childNode.GetLines(ref list);
                }
            }
        }

        protected virtual void WriteStream(ref StreamWriter writer)
        {
            if (!NoOutput)
            {
                writer.WriteLine(RawLine);
            }
            if (SupportsChildren)
            {
                foreach (RefsNode childNode in GetChildren())
                {
                    childNode.WriteStream(ref writer);
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

        protected virtual void ClearCachedData()
        {
            _inOptionalBlock = null;
            _pathSource = null;
            for (int i = 0; i < Children.Count; i++)
            {
                RefsNode child = Children[i];
                child.ClearCachedData();
            }
        }
        #endregion

        #region IList<T>
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
        public int Count => Children.Count;
        public bool IsReadOnly => Children.IsReadOnly;

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
        /// <exception cref="NotSupportedException">Thrown when children aren't supported on this <see cref="RefsNode"/></exception>
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
        /// Removes the child at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotSupportedException">Thrown when children aren't supported on this <see cref="RefsNode"/></exception>
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
        /// <summary>
        /// Removes all children from the specified <see cref="RefsNode"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when children aren't supported on this <see cref="RefsNode"/></exception>
        public virtual void Clear()
        {
            foreach (RefsNode child in Children)
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

        /// <summary>
        /// Removes the first occurence of the specified <see cref="RefsNode"/>.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when children aren't supported on this <see cref="RefsNode"/></exception>
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

        #endregion
    }

}
