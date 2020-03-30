using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public static class RefsNodeExtensions
    {
        public static void AddNode(this IEnumerable<RefsNode> refsNodes, RefsNode node, bool optional = false)
        {
            if (refsNodes == null)
                throw new ArgumentNullException($"{nameof(refsNodes)} cannot be null for {nameof(AddNode)}.");
            if (node == null)
                throw new ArgumentNullException($"{nameof(node)} cannot be null for {nameof(AddNode)}.");
            RefsNode previousNode = null;
            foreach (var rootNode in refsNodes.ToArray())
            {

            }
        }
    }
}
