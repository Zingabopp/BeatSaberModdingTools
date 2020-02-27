using BeatSaberModdingTools.BuildTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSMT_Tests.BuildTools
{
    [TestClass]
    public class RefsNode_Tests
    {
        private readonly string DataPath = Path.Combine("Data", "BuildTools");

        [TestMethod]
        public void TryGetReference_Exists()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            string fileName = "Main.dll";
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode rootNodes = reader.ReadFile();
            RefsNode testNode = rootNodes[0];
            Assert.IsTrue(testNode.TryGetReference(fileName, out FileNode fileNode));
            fileNode.Alias = "NewMain.dll";
            Console.WriteLine(string.Join("\n", testNode.GetLines()));
        }

        [TestMethod]
        public void TryGetReference_NonExistant()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            string fileName = "NonExistant.dll";
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode rootNodes = reader.ReadFile();
            RefsNode testNode = rootNodes[0];
            int childCount = testNode.Count;
            testNode.Insert(childCount, new FileNode("TestEnd.dll"));
            testNode.Insert(0, new FileNode("TestStart.dll"));
            testNode.Insert(3, new FileNode("TestMiddle.dll"));
            Assert.IsFalse(testNode.TryGetReference(fileName, out FileNode fileNode));
            Assert.IsNull(fileNode);
        }

        [TestMethod]
        public void InsertChild()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode rootNode = reader.ReadFile();
            RefsNode testNode = rootNode.First();
            int middleIndex = 3;
            RefsNode first = new FileNode("TestStart.dll");
            RefsNode middle = new FileNode("TestMiddle.dll");
            RefsNode last = new FileNode("TestEnd.dll");
            testNode.Insert(testNode.Count, last);
            testNode.Insert(0, first);
            testNode.Insert(middleIndex, middle);
            Assert.AreEqual(first, testNode[0]);
            Assert.AreEqual(middle, testNode[middleIndex]);
            Assert.AreEqual(last, testNode[testNode.Count - 1]);
        }

        [TestMethod]
        public void AddChild()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode rootNodes = reader.ReadFile();
            RefsNode testNode = rootNodes[0];
            RefsNode last = new FileNode("TestEnd.dll");
            testNode.Add(last);
            Assert.AreEqual(last, testNode[testNode.Count - 1]);
        }

        [TestMethod]
        public void GetFilename()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            string fileName = "Main.dll";
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode rootNodes = reader.ReadFile();
            RefsNode testNode = rootNodes[0];

            Assert.IsTrue(testNode.TryGetReference(fileName, out FileNode fileNode));

            Console.WriteLine(fileNode.GetRelativePath());
        }

        [TestMethod]
        public void GetRelativePath()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            string fileName = "Main.dll";
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode rootNodes = reader.ReadFile();
            RefsNode testNode = rootNodes[0];
            Assert.IsTrue(testNode.TryGetReference(fileName, out FileNode fileNode));

            Console.WriteLine(fileNode.GetRelativePath());
        }

        [TestMethod]
        public void GetFilenameAfterRelativePath()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            string fileName = "Main.dll";
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode rootNodes = reader.ReadFile();
            RefsNode testNode = rootNodes[0];
            Assert.IsTrue(testNode.TryGetReference(fileName, out FileNode fileNode));
            Console.WriteLine(fileNode.GetRelativePath());
            Assert.AreEqual(fileName, fileNode.GetFilename());
        }

        [TestMethod]
        public void GetLines()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode root = reader.ReadFile();
            Assert.IsTrue(root.Count > 0);
            foreach (var line in root.GetLines())
            {
                Console.WriteLine(line);
            }
        }

        [TestMethod]
        public void ReadFileAndCompare()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode things = reader.ReadFile();
            Assert.IsTrue(things.Count > 0);
            string text = string.Empty;
            string[] stringList = things.GetLines();

            text = string.Join("\n", stringList);
            //Assert.AreEqual(File.ReadAllText(refsText), text);
            string line;
            int lineNumber = 0;
            using (StreamReader streamReader = new StreamReader(refsText))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (lineNumber < stringList.Length)
                    {
                        Console.WriteLine(stringList[lineNumber]);
                        Assert.AreEqual(line, stringList[lineNumber]);
                    }
                    else
                        Assert.Fail("Different number of lines");
                    lineNumber++;
                }
            }
            Assert.AreEqual(lineNumber, stringList.Length);
        }

        [TestMethod]
        public void GetFileString()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            BuildToolsRefsParser reader = new BuildToolsRefsParser(refsText);
            Assert.IsTrue(reader.FileExists);
            RootNode things = reader.ReadFile();
            Assert.IsTrue(things.Count > 0);
            string[] parsedText = things.GetFileString().Split(new char[] { '\n' }, StringSplitOptions.None);
            string line;
            int lineNumber = 0;
            using (StreamReader streamReader = new StreamReader(refsText))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (lineNumber < parsedText.Length)
                    {
                        Console.WriteLine(parsedText[lineNumber]);
                        Assert.AreEqual(line, parsedText[lineNumber]);
                    }
                    else
                        Assert.Fail("Different number of lines");
                    lineNumber++;
                }
            }
            //Assert.AreEqual(originalText, parsedText);
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
