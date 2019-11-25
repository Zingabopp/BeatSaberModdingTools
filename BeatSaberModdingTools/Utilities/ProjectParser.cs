using BeatSaberModdingTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BeatSaberModdingTools.Utilities
{
    public class ProjectParser
    {
        public ProjectModel Project { get; private set; }
        public bool IsParsed { get; private set; }
        public string FilePath { get; private set; }
        public XDocument Document { get; private set; }
        public ProjectParser(string filePath)
        {
            FilePath = filePath;
        }
        public bool TryParseProjectFile(out ProjectModel project)
        {
            project = null;
            if (string.IsNullOrEmpty(FilePath)) return false;
            if (!File.Exists(FilePath)) return false;
            try
            {
                Document = XDocument.Load(FilePath);
                // --- To Read & Store ---
                // <PropGroup>BeatSaberDir, Some BSMT flag
                // <ItemGroup>References
                // PostBuild
                // <ItemGroup><EmbeddedResource Include=Manifest
                // <PropGroup><PostBuildEvent>
                // --- Location to Store ---
                // Definitions prop group: to insert BeatSaberDir
                // References item group: to work with future reference add/remover 
                //    (or figure out how to do it through VS.)
                // PostBuildEvent? Probably better to do it in the csproj.user.
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
