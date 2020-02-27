using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public struct FileEntry
    {
        public readonly string Fullname;
        public readonly string Alias;
        public readonly string PathSource;
        public readonly FileFlag Flag;
        public FileEntry(string fullName, FileFlag flag, string alias = null, string pathSource = null)
        {
            Fullname = fullName;
            Alias = alias;
            Flag = flag;
            if (string.IsNullOrEmpty(pathSource))
                PathSource = CommandNode.DefaultFromSource;
            else
                PathSource = pathSource;
        }
    }
}
