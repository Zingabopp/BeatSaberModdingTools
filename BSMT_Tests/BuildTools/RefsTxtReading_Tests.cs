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
        public void ReadFileAndCompare()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode things = reader.ReadFile();
            Assert.IsTrue(things.Count > 0);
            string text = string.Empty;
            List<string> stringList = new List<string>();
            foreach (RefsNode rootNode in things)
            {
                stringList.AddRange(rootNode.GetLines());
            }
            text = string.Join("\n", stringList);
            //Assert.AreEqual(File.ReadAllText(refsText), text);
            string line;
            int lineNumber = 0;
            using (StreamReader streamReader = new StreamReader(refsText))
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
            if (node is FileNode leafNode)
            {
                Console.WriteLine(node.NodeDepth.ToString("00") + " | " + node.RawLine + " | " + leafNode.GetFileEntry());
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
