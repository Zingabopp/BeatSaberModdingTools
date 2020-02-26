using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public enum FileFlag
    {
        Unknown = 0,
        Empty = 1,
        Virtualize = 2,
        Native = 3
    }
    public struct FileEntry
    {
        private const string AliasFlag = "alias=";
        public readonly string File;
        public readonly string Alias;
        public readonly FileFlag Flag;
        public readonly int Line;
        public readonly bool Optional;

        public FileEntry(string file, int line, bool optional)
        {
            string[] fileParts = file.Split('?');
            File = fileParts[0];
            Alias = null;
            Flag = FileFlag.Empty;
            for (int i = 1; i < fileParts.Length; i++)
            {
                if (string.IsNullOrEmpty(fileParts[i]))
                    continue;
                if (fileParts[i].StartsWith("virt"))
                    Flag = FileFlag.Virtualize;
                else if (fileParts[i].StartsWith("native"))
                    Flag = FileFlag.Native;
                else if (fileParts[i].StartsWith(AliasFlag))
                {
                    Alias = fileParts[i].Substring(AliasFlag.Length);
                }
                else
                    Console.WriteLine($"WARNING: ({line}): Unrecognized file flag: {fileParts[i]}");
            }

            Line = line;
            Optional = optional;
        }

        public FileEntry(string file, int line, bool optional, FileFlag flag, string alias)
        {
            File = file;
            Line = line;
            Flag = flag;
            Alias = alias;
            Optional = optional;
        }

        public override string ToString()
        {
            return $"({Line.ToString("00")}): {File}{(Optional ? "*" : string.Empty)} |{Flag}|{(string.IsNullOrEmpty(Alias) ? string.Empty : $" -> {Alias}")}";
        }
    }

    public class BuildToolsRefsParser
    {
        private string _refsFilePath;
        protected HashSet<string> RequiredReferences { get; private set; }
        protected HashSet<string> OptionalReferences { get; private set; }
        public bool FileExists => File.Exists(_refsFilePath);
        public BuildToolsRefsParser(string refsFilePath)
        {
            _refsFilePath = refsFilePath;
            RequiredReferences = new HashSet<string>();
            OptionalReferences = new HashSet<string>();
        }

        public RootNode Root { get; protected set; }

        public RootNode ReadFile()
        {
            if (!FileExists)
                return null;
            RootNode rootNodes = new RootNode();
            CommandNode currentRoot = null;
            RefsNode currentNode = null;
            string depsFile = File.ReadAllText(_refsFilePath);
            var lineNo = 0;
            int currentLevel;
            int nextLevel;
            string[] allLines = depsFile.Split(new[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.None);

            string currentLine = allLines[0];
            string nextLine;
            for (int i = 0; i < allLines.Length; i++) 
            {
                if (i + 1 < allLines.Length)
                    nextLine = allLines[i + 1];
                else
                    nextLine = string.Empty;
                var currentParts = currentLine.Split('"');
                var nextParts = nextLine?.Split('"') ?? Array.Empty<string>();
                var path = currentParts.Last();
                currentLevel = currentParts.Length - 2;
                nextLevel = Math.Max(nextParts.Length - 2, 0);
                if (path.StartsWith("::") || path == string.Empty)
                { // pseudo-command
                    if (currentNode is CommandNode currentCmd && currentCmd.Command == CommandNode.CommandType.StartOptionalBlock)
                    {
                        currentNode = new CommandNode(currentLine);
                        currentRoot.Add(currentNode);
                    }
                    else
                    {
                        currentRoot = new CommandNode(currentLine);
                        currentNode = currentRoot;
                        rootNodes.Add(currentRoot);
                    }
                    currentLine = nextLine;
                    continue;
                }

                if (currentLevel < nextLevel)
                {
                    LeafNode toAdd = new LeafNode(currentLine);
                    currentNode.Add(toAdd);
                    currentNode = toAdd;
                }
                else if (currentLevel == nextLevel)
                    currentNode.Add(new FileNode(currentLine));
                else if (currentLevel > nextLevel)
                {
                    currentNode.Add(new FileNode(currentLine));
                    while (currentNode.NodeDepth > nextLevel)
                        currentNode = currentNode.Parent;
                }

                lineNo++;
                currentLine = nextLine;
            }
            Root = rootNodes;
            return rootNodes;
        }
    }
}
