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
        private const string Cmd_FromBsInstall = @"::from ./bsinstalldir.txt";
        private const string Cmd_StartOpt = @"::startopt";
        private const string Cmd_EndOpt = @"::endopt";
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

        public List<CommandNode> Root { get; protected set; }

        public List<CommandNode> ReadFile()
        {
            if (!FileExists)
                return null;
            List<CommandNode> rootNodes = new List<CommandNode>();
            CommandNode currentRoot = null;
            RefsNode currentNode = null;
            string depsFile = File.ReadAllText(_refsFilePath);
            var optBlock = false;
            var lineNo = 0;
            int currentLevel;
            int nextLevel;
            bool repeat;
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
                        currentNode = currentRoot.AddChild(new CommandNode(currentLine));
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
                    currentNode = currentNode.AddChild(new LeafNode(currentLine));
                else if (currentLevel == nextLevel)
                    currentNode.AddChild(new FileNode(currentLine));
                else if (currentLevel > nextLevel)
                {
                    currentNode.AddChild(new FileNode(currentLine));
                    while (currentNode.NodeDepth > nextLevel)
                        currentNode = currentNode.Parent;
                }

                lineNo++;
                currentLine = nextLine;
            }
            Root = rootNodes;
            return rootNodes;
        }

        public FileEntry[] ReadFileOld()
        {
            if (!FileExists)
                return Array.Empty<FileEntry>();

            var files = new List<FileEntry>();
            //StreamReader file = new StreamReader(_refsFilePath);
            string depsFile = File.ReadAllText(_refsFilePath);
            var stack = new Stack<string>();

            void Push(string val)
            {
                string pre = "";
                if (stack.Count > 0)
                    pre = stack.First();
                stack.Push(pre + val);
            }
            string Pop() => stack.Pop();
            string Replace(string val)
            {
                var v2 = Pop();
                Push(val);
                return v2;
            }
            var optBlock = false;
            var lineNo = 0;
            //string line;
            foreach (string line in depsFile.Split(new[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split('"');
                var path = parts.Last();
                var level = parts.Length - 2;

                if (path.StartsWith("::"))
                { // pseudo-command
                    parts = path.Split(' ');
                    var command = parts[0].Substring(2);
                    parts = parts.Skip(1).ToArray();
                    var arglist = string.Join(" ", parts);
                    if (command == "from")
                    { // an "import" type command
                        string filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), arglist));
                        try
                        {
                            path = File.ReadAllText(filePath);
                        }
                        catch (FileNotFoundException e)
                        {
                            var errorStrength = optBlock ? "WARNING" : "ERROR";
                            Console.WriteLine($"{errorStrength}: {Path.GetFullPath(_refsFilePath)}({lineNo}): File doesn't exist: {filePath}");
                            path = "$\"Invalid Path";
                        }
                        catch (Exception e)
                        {
                            var errorStrength = optBlock ? "WARNING" : "ERROR";
                            Console.WriteLine($"{errorStrength}: {Path.GetFullPath(_refsFilePath)}({lineNo}): Error resolving import {filePath}: {e}");
                            path = "$\"Invalid Path";
                        }
                    }
                    else if (command == "prompt")
                    {
                        Console.Write(arglist);
                        path = Console.ReadLine();
                    }
                    else if (command == "startopt")
                    {
                        optBlock = true;
                        goto continueTarget;
                    }
                    else if (command == "endopt")
                    {
                        optBlock = false;
                        goto continueTarget;
                    }
                    else
                    {
                        path = "";
                        Console.WriteLine($"{Path.GetFullPath(_refsFilePath)}({lineNo}): error: Invalid command {command}");
                    }
                    continue;
                }

                if (level > stack.Count - 1)
                    Push(path);
                else if (level == stack.Count - 1)
                    files.Add(new FileEntry(Replace(path), lineNo, optBlock));
                else if (level < stack.Count - 1)
                {
                    files.Add(new FileEntry(Pop(), lineNo, optBlock));
                    while (level < stack.Count)
                        Pop();
                    Push(path);
                }

            continueTarget:
                lineNo++;
            }
            files.Add(new FileEntry(Pop(), lineNo, optBlock));
            return files.ToArray();
        }
    }
}
