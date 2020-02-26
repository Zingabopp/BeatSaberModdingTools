using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public class RootNode : RefsNode
    {
        public override RefsNodesType NodeType => RefsNodesType.Root;

        public override string RawLine => string.Empty;

        public override int NodeDepth => 0;

        public override bool SupportsChildren => true;

        public override bool TryGetReference(string fileName, out FileNode fileNode)
        {
            foreach (var node in Children)
            {
                if (TryGetReference(fileName, out fileNode))
                    return true;
            }
            fileNode = null;
            return false;
        }
    }
}
