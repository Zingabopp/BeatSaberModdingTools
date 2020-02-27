using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public class CommandNode : RefsNode
    {
        public static readonly string DefaultFromSource = "./bsinstalldir.txt";
        public static readonly string PromptSourceTag = "PROMPT";
        public enum CommandType
        {
            None = 0,
            From = 1,
            Prompt = 2,
            OptionalBlock = 3,
            EmptyLine
        }

        public override RefsNodesType NodeType => RefsNodesType.Command;
        public override string RawLine
        {
            get
            {
                if (Command == CommandType.EmptyLine)
                    return string.Empty;
                return $"::{ConvertToString(Command)}{(string.IsNullOrEmpty(CommandData) ? string.Empty : " " + CommandData)}";
            }
        }

        public override bool SupportsChildren => !(Command == CommandType.EmptyLine);

        public override bool TryGetReference(string fileName, out FileNode fileNode)
        {
            if (SupportsChildren)
            {
                foreach (var child in Children)
                {
                    if (child.TryGetReference(fileName, out FileNode foundNode))
                    {
                        fileNode = foundNode;
                        return true;
                    }
                }
            }
            fileNode = null;
            return false;
        }

        public CommandType Command { get; protected set; }
        public string CommandData { get; protected set; }

        public override int NodeDepth => 0;

        public static CommandType ConvertFromString(string command)
        {
            if (command == "from")
            {
                return CommandType.From;
            }
            else if (command == "prompt")
            {
                return CommandType.Prompt;
            }
            else if (command == "startopt")
            {
                return CommandType.OptionalBlock;
            }
            //else if (command == "endopt")
            //{
            //    return CommandType.EndOptionalBlock;
            //}
            else
                return CommandType.None;
        }

        public static string ConvertToString(CommandType command)
        {
            switch (command)
            {
                case CommandType.From:
                    return "from";
                case CommandType.Prompt:
                    return "prompt";
                case CommandType.OptionalBlock:
                    return "startopt";
                case CommandType.EmptyLine:
                    return string.Empty;
                default:
                    return "INVALID";
            }
        }

        protected override void GetLines(ref List<string> list)
        {
            base.GetLines(ref list);
            if (Command == CommandType.OptionalBlock)
                list.Add("::endopt");
        }

        protected override void WriteStream(ref StreamWriter writer)
        {
            base.WriteStream(ref writer);
            if (Command == CommandType.OptionalBlock)
            {
                writer.WriteLine("::endopt");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawLine"></param>
        /// <exception cref="ArgumentException"></exception>
        public CommandNode(string rawLine)
        {
            if (string.IsNullOrEmpty(rawLine))
            {
                Command = CommandType.EmptyLine;
                CommandData = string.Empty;
                return;
            }
            //Children = new List<RefsNode>();
            string[] parts;
            if (rawLine.StartsWith("::"))
            {
                parts = rawLine.Split(' ');
                string command = parts[0].Substring(2);
                parts = parts.Skip(1).ToArray();
                string arglist = string.Join(" ", parts);
                Command = ConvertFromString(command);
                if (Command == CommandType.None) throw new ArgumentException($"Invalid command: {rawLine}");
                CommandData = arglist;
            }
            else
                throw new ArgumentException($"Invalid command: {rawLine}");
        }
        public CommandNode(CommandType commandType, string commandData)
        {
            //Children = new List<RefsNode>();
            Command = commandType;
            CommandData = commandData;
        }
    }
}
