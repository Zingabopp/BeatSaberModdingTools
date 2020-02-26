using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public class FileNode : LeafNode
    {
        private const string AliasFlag = "alias=";
        public string File { get; private set; }
        public string Alias { get; set; }
        public FileFlag Flag { get; set; }

        public override RefsNodesType NodeType => RefsNodesType.File;

        public override string RawLine
        {
            get
            {
                string flag;
                switch (Flag)
                {
                    case FileFlag.Virtualize:
                        flag = "?virt";
                        break;
                    case FileFlag.Native:
                        flag = "?native";
                        break;
                    default:
                        flag = string.Empty;
                        break;
                }
                string alias = string.IsNullOrEmpty(Alias) ? string.Empty : "?" + AliasFlag + Alias;
                string data = $"{File}{flag}{alias}";
                return data.PadLeft(data.Length + LeafLevel, '"');
            }
        }

        public override bool SupportsChildren => false;

        public FileEntry GetFileEntry()
        {
            if (!IsFile)
                throw new InvalidOperationException("Cannot create a file entry from a LeafNode that is not a file.");
            int lineNumber = 1;
            RefsNode next = Parent;
            bool inOptionalBlock = false;
            while(next != null)
            {
                if (next is CommandNode cmdNode && cmdNode.Command == CommandNode.CommandType.StartOptionalBlock)
                    inOptionalBlock = true;
                lineNumber++;
                next = next.Parent;
            }
            return new FileEntry(string.Concat(GetPathParts()), lineNumber, inOptionalBlock);
        }

        public FileNode(string rawLine)
            : base(rawLine)
        {
            SetData(rawLine);
        }

        public FileNode(int leafLevel, string leafData)
            : base(leafLevel, leafData)
        {
            SetData(leafData.PadLeft(leafData.Length + leafLevel, '"'));
        }

        private void SetData(string data)
        {
            string[] fileParts = data.Split('?');
            File = fileParts[0].TrimStart('"');
            Alias = null;
            Flag = FileFlag.Empty;
            for (int i = 1; i < fileParts.Length; i++)
            {
                if (string.IsNullOrEmpty(fileParts[i]))
                    continue;
                if (fileParts[i].StartsWith("virt"))
                    Flag = FileFlag.Virtualize;
                else if (fileParts[i].StartsWith(AliasFlag))
                {
                    Alias = fileParts[i].Substring(AliasFlag.Length);
                }
                else
                    Flag = FileFlag.Unknown;
            }
        }
    }
}
