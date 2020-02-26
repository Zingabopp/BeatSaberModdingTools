using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeatSaberModdingTools.BuildTools;
using System.IO;

namespace BSMT_Tests.BuildTools
{
    [TestClass]
    public class CommandNode_Tests
    {
        private readonly string DataPath = Path.Combine("Data", "BuildTools");
        [TestMethod]
        public void CommandEquality()
        {
            string rawLine = "::from ./bsinstalldir.txt";
            CommandNode.CommandType commandType = CommandNode.CommandType.From;
            string commandData = "./bsinstalldir.txt";
            CommandNode node1 = new CommandNode(rawLine);
            CommandNode node2 = new CommandNode(commandType, commandData);
            Assert.AreEqual(node1.RawLine, node2.RawLine);

            rawLine = "::startopt";
            commandType = CommandNode.CommandType.StartOptionalBlock;
            commandData = "";
            node1 = new CommandNode(rawLine);
            node2 = new CommandNode(commandType, commandData);
            Assert.AreEqual(node1.RawLine, node2.RawLine);

            rawLine = "::endopt";
            commandType = CommandNode.CommandType.EndOptionalBlock;
            commandData = "";
            node1 = new CommandNode(rawLine);
            node2 = new CommandNode(commandType, commandData);
            Assert.AreEqual(node1.RawLine, node2.RawLine);
        }

        [TestMethod]
        public void BuildFromFile()
        {
            string refsText = Path.GetFullPath(Path.Combine(DataPath, "refs.txt"));
            string rawLine = "::from ./bsinstalldir.txt";
            CommandNode root = new CommandNode(rawLine);
            LeafNode childToAdd = new LeafNode(@"""Beat Saber_Data/");
            root.Add(childToAdd);
            childToAdd = new LeafNode(@"""""Managed/");
            root.Add(childToAdd);
            childToAdd.Add(new FileNode(@"""""""Unity.TextMeshPro.dll?virt?alias=UnityAlias.TextMeshPro.dll"));
            childToAdd.Add(new FileNode(@"""""""UnityEngine.dll"));
            PrintChildren(root);
        }

        public void PrintChildren(RefsNode node)
        {
            if (node is FileNode leafNode)
            {
                Console.WriteLine(node.RawLine + " | " + leafNode.GetFileEntry());
            }
            else
                Console.WriteLine(node.RawLine);
            if (!node.SupportsChildren)
                return;
            foreach (var childNode in node.GetChildren())
            {
                PrintChildren(childNode);
            }
        }
    }
}
