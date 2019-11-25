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
        public static void IterateThroughAllElements(this XDocument doc, Func<XElement, int, bool> elementAction)
        {
            if (doc != null && elementAction != null)
            {
                foreach (var element in doc.Elements())
                {
                    doIterateElement(element, elementAction, 0);
                }
            }
        }


        private static void doIterateElement(XElement element, Func<XElement, int, bool> elementAction, int depth)
        {
            if (elementAction.Invoke(element, depth))
            {
                foreach (var child in element.Elements())
                {
                    doIterateElement(child, elementAction, depth + 1);
                }
            }
        }

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

        public static bool TryGetAttribute(this XElement element, string attributeName, out string value)
        {
            value = null;
            if (element == null || string.IsNullOrEmpty(attributeName)) return false;

            value = element.Attributes().Where(a => a.Name.LocalName == attributeName).Select(a => a.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(value))
                return true;
            return false;
        }

        public static bool TryFindFirstChild(this XElement element, string localName, out XElement childElement)
        {
            childElement = element.Elements().Where(e => e.Name.LocalName == localName).FirstOrDefault();
            if (childElement == null)
                return false;
            return true;
        }
    }
}
