using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeatSaberModdingTools.BuildTools;
using System.IO;
using System.Collections.Generic;

namespace BSMT_Tests.BuildTools
{
    [TestClass]
    public class RefsTxtReading
    {
        private readonly string DataPath = Path.Combine("Data", "BuildTools");
        [TestMethod]
        public void TestMethod1()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            var reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            var things = reader.ReadFile();
            Assert.IsTrue(things.Count > 0);
            string text = string.Empty;
            List<string> stringList = new List<string>();
            foreach (var rootNode in things)
            {
                stringList.AddRange(GetLines(rootNode));
            }
            text = string.Join("\n", stringList);
            //Assert.AreEqual(File.ReadAllText(refsText), text);
            string line;
            int lineNumber = 0;
            using (var streamReader = new StreamReader(refsText))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (lineNumber < stringList.Count)
                    {
                        Console.WriteLine(stringList[lineNumber]);
                        Assert.AreEqual(line, stringList[lineNumber]);
                    }
                    else
                        Assert.Fail("Different number of lines");
                    lineNumber++;
                }
            }
            Assert.AreEqual(lineNumber, stringList.Count);
        }

        public string[] GetLines(RefsNode node)
        {
            List<string> lines = new List<string>();
            return GetLines(node, ref lines).ToArray();
        }

        private List<string> GetLines(RefsNode node, ref List<string> list)
        {
            list.Add(node.RawLine);
            if (node.SupportsChildren)
            {
                foreach (var childNode in node.GetChildren())
                {
                    GetLines(childNode, ref list);
                }
            }
            return list;
        }

        public void PrintChildren(RefsNode node)
        {
            if (node is FileNode leafNode)
            {
                Console.WriteLine(node.NodeDepth.ToString("00") + " | " + node.RawLine + " | " + leafNode.GetFileEntry());
            }
            else
                Console.WriteLine(node.NodeDepth.ToString("00") + " | " + node.RawLine);
            if (!node.SupportsChildren)
                return;
            foreach (var childNode in node.GetChildren())
            {
                PrintChildren(childNode);
            }
        }
    }
}
