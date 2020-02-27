using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public class RootNode : RefsNode
    {
        public override RefsNodesType NodeType => RefsNodesType.Root;

        public override string RawLine => string.Empty;

        public override int NodeDepth => 0;

        public override bool SupportsChildren => true;

        public override bool TryGetReference(string fileName, out FileNode fileNode)
        {
            foreach (var node in Children)
            {
                if (TryGetReference(fileName, out fileNode))
                    return true;
            }
            fileNode = null;
            return false;
        }

        protected override bool NoOutput => true;

        public string GetFileString()
        {
            return string.Join("\n", GetLines());
        }

        public void WriteToStream(Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(stream, encoding, 1024, true);
                writer.AutoFlush = true;
                WriteStream(ref writer);
            }
            finally
            {
                if(writer != null)
                    writer.Dispose();
            }
        }

        public void WriteToFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                WriteToStream(fs);
            }
        }

        public override string ToString()
        {
            return $"<Root> | {Children.Count} children";
        }
    }
}
