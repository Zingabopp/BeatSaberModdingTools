using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BeatSaberModdingTools.Utilities
{
    public static class XmlExtensions
    {
        public static void IterateThroughAllNodes(
            this XDocument doc,
            Action<XObject, int> elementVisitor)
        {
            if (doc != null && elementVisitor != null)
            {
                foreach (XNode node in doc.Nodes())
                {
                    doIterateNode(node, elementVisitor, 0);
                }
            }
        }

        private static void doIterateNode(
            XObject node,
            Action<XObject, int> elementVisitor, int depth)
        {
            elementVisitor(node, depth);

            if (node is XContainer container)
            {
                foreach (XNode childNode in container.Nodes())
                {
                    doIterateNode(childNode, elementVisitor, depth + 1);
                }
            }
        }
    }
}
