using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public class LeafNode : RefsNode
    {
        public override RefsNodesType NodeType => RefsNodesType.Leaf;

        public override string RawLine => LeafData.PadLeft(LeafData.Length + LeafLevel, '"');

        public override bool SupportsChildren => true;
        public bool IsFile => NodeType == RefsNodesType.File;
        public int LeafLevel => NodeDepth;
        public string LeafData { get; protected set; }

        public override int NodeDepth
        {
            get
            {
                int depth = 1;
                RefsNode next = Parent;
                while (next.GetType() == typeof(LeafNode))
                {
                    depth++;
                    next = next.Parent;
                }
                return depth;
            }
        }

        public string[] GetPathParts()
        {
            Stack<string> stack = new Stack<string>();
            stack.Push(LeafData);
            GetParentPathParts(ref stack);
            string[] retAry = new string[stack.Count];
            int stackSize = stack.Count;
            for(int i = 0; i < stackSize; i++)
            {
                retAry[i] = stack.Pop();
            }
            return retAry;
        }

        protected void GetParentPathParts(ref Stack<string> stack)
        {
            if(Parent is LeafNode parentLeaf)
            {
                stack.Push(parentLeaf.LeafData);
                parentLeaf.GetParentPathParts(ref stack);
            }
        }

        public LeafNode(string rawLine)
        {
            //Children = new List<RefsNode>();
            var parts = rawLine.Split('"');
            //LeafLevel = parts.Length - 1;
            LeafData = parts.Last();
        }

        public LeafNode(int leafLevel, string leafData)
        {
            //Children = new List<RefsNode>();
            LeafData = leafData;
        }
    }
}
