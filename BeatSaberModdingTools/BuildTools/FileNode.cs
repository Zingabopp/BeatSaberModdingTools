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

    public class FileNode : LeafNode
    {
        private const string AliasFlag = "alias=";
        public string File { get; private set; }
        public string Alias { get; set; }
        public FileFlag Flag { get; set; }
        protected string Filename;

        public override string GetRelativePath()
        {
            string name = File.TrimStart('"');
            if (Parent is LeafNode leafNode)
                name = leafNode.GetRelativePath() + name;
            RelativePath = name;
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preferAlias"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when the file doesn't have a valid path in the tree.</exception>
        public string GetFilename(bool preferAlias = false)
        {
            if (preferAlias && !string.IsNullOrEmpty(Alias))
                return Alias;
            if (!string.IsNullOrEmpty(Filename))
                return Filename;
            if (!string.IsNullOrEmpty(RelativePath))
                return Path.GetFileName(RelativePath);
            string name = File;
            RefsNode next = Parent;
            while (!(name.Contains(Path.DirectorySeparatorChar) || name.Contains(Path.AltDirectorySeparatorChar)))
            {
                if (next is LeafNode leaf)
                {
                    name = leaf.LeafData + name;
                    next = next.Parent;
                }
                else
                    break;
            }
            //if (name.Contains(Path.DirectorySeparatorChar) || name.Contains(Path.AltDirectorySeparatorChar))
            //{
            try
            {
                Filename = Path.GetFileName(name);
                return Filename;
            }
            catch (ArgumentException)
            {

            }
            //}
            throw new InvalidOperationException($"FileNode '{File}' doesn't appear to be a valid path");
        }

        public override bool TryGetReference(string fileName, out FileNode fileNode)
        {
            if (fileName.Equals(GetFilename(), StringComparison.Ordinal))
            {
                fileNode = this;
                return true;
            }
            else
            {
                fileNode = null;
                return false;
            }
        }

        protected override void ClearCachedData()
        {
            base.ClearCachedData();
            Filename = null;
        }

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
        private bool? _optional;
        public bool Optional
        {
            get
            {
                if (_optional != null)
                    return _optional ?? false;
                _optional = false;
                RefsNode next = Parent;
                while (next != null)
                {
                    if (next is CommandNode cmdNode && cmdNode.Command == CommandNode.CommandType.OptionalBlock)
                    {
                        _optional = true;
                        break;
                    }
                    next = next.Parent;
                }
                return _optional ?? false;
            }
        }

        public FileNode(string rawLine)
            : base(rawLine)
        {
            SetData(rawLine);
        }

        private void SetData(string data)
        {
            string[] fileParts = data.Split('?');
            File = fileParts[0].Substring(Math.Max(fileParts[0].LastIndexOf('"') + 1, 0));
            Alias = null;
            Flag = FileFlag.Empty;
            for (int i = 1; i < fileParts.Length; i++)
            {
                if (string.IsNullOrEmpty(fileParts[i]))
                    continue;
                if (fileParts[i].StartsWith("virt"))
                    Flag = FileFlag.Virtualize;
                else if (fileParts[i].StartsWith(AliasFlag))
                    Alias = fileParts[i].Substring(AliasFlag.Length);
                else
                    Flag = FileFlag.Unknown;
            }
        }

        public override string ToString()
        {
            return RawLine;
        }
    }
}
