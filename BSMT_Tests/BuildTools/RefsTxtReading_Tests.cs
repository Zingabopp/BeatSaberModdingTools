using BeatSaberModdingTools.BuildTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSMT_Tests.BuildTools
{
    [TestClass]
    public class RefsTxtReading_Tests
    {
        private readonly string DataPath = Path.Combine("Data", "BuildTools");
        [TestMethod]
        public void PrintInfo()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode root = reader.ReadFile();
            Assert.IsTrue(root.Count > 0);
            PrintChildren(root);

        }

        public string[] GetLines(RefsNode node)
        {
            List<string> lines = new List<string>();
            return GetLines(node, ref lines).ToArray();
        }

        private List<string> GetLines(RefsNode node, ref List<string> list)
        {
            string lineToAdd = node.RawLine;
            if (node is FileNode fileNode)
                lineToAdd = lineToAdd + " | " + fileNode.GetFilename(true);
            list.Add(lineToAdd);
            if (node.SupportsChildren)
            {
                foreach (RefsNode childNode in node.GetChildren())
                {
                    GetLines(childNode, ref list);
                }
            }
            return list;
        }

        public void PrintChildren(RefsNode node)
        {
            if (node is FileNode fileNode)
            {
                Console.WriteLine(node.NodeDepth.ToString("00") + " | " + node.RawLine + " | " + fileNode);
            }
            else
                Console.WriteLine(node.NodeDepth.ToString("00") + " | " + node.RawLine);
            if (!node.SupportsChildren)
                return;
            foreach (RefsNode childNode in node.GetChildren())
            {
                PrintChildren(childNode);
            }
        }
    }


}
