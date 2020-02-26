using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatSaberModdingTools.BuildTools
{
    public enum RefsNodesType
    {
        Unknown = 0,
        Command = 1,
        Leaf = 2,
        File = 3
    }
    public abstract class RefsNode
    {
        public abstract RefsNodesType NodeType { get; }
        public abstract string RawLine { get; }
        public virtual RefsNode Parent { get; protected set; }
        public abstract int NodeDepth { get; }
        public abstract bool SupportsChildren { get; }

        public virtual RefsNode[] GetChildren()
        {
            return (!SupportsChildren || Children.Count == 0) ? Array.Empty<RefsNode>() : Children.ToArray();
        }
        protected RefsNode()
        {
            Children = SupportsChildren ? new List<RefsNode>() : null;
        }
        protected List<RefsNode> Children { get; }

        protected virtual void SetParent(RefsNode newParent)
        {
            if (Parent == newParent)
                return;
            if (Parent != null)
                Parent.Children.Remove(this);
            Parent = newParent;
            if (!Parent.Children.Contains(this))
                Parent.Children.Add(this);
        }

        /// <summary>
        /// Adds the provided <see cref="RefsNode"/> as a child. Sets <paramref name="child"/>'s <see cref="Parent"/> to this node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns>The child node that was added.</returns>
        /// <exception cref="NotSupportedException">Thrown when children aren't supported on this <see cref="RefsNode"/></exception>
        public virtual T AddChild<T>(T child) where T : RefsNode
        {
            if (!SupportsChildren)
                throw new NotSupportedException($"RefsNode type {GetType().Name} does not support children");
            child.SetParent(this);
            return child;
        }

        public override string ToString()
        {
            return RawLine;
        }
    }

}
