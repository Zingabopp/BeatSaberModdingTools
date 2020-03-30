using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    
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
            RootNode rootNode = new RootNode();
            CommandNode currentCommand = null;
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
                    if (currentNode is CommandNode currentCmd && currentCmd.Command == CommandNode.CommandType.OptionalBlock)
                    {
                        currentNode = new CommandNode(currentLine);
                        currentCommand.Add(currentNode);
                    }
                    else if(path.StartsWith("::endopt"))
                    {
                        while(currentNode.Parent != null)
                        {
                            currentNode = currentNode.Parent;
                            if(currentNode is CommandNode commandNode && commandNode.Command == CommandNode.CommandType.OptionalBlock)
                            {
                                currentNode = currentNode.Parent;
                                break;
                            }
                        }
                    }
                    else
                    {
                        currentCommand = new CommandNode(currentLine);
                        currentNode = currentCommand;
                        rootNode.Add(currentCommand);
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
            Root = rootNode;
            return rootNode;
        }
    }
}
