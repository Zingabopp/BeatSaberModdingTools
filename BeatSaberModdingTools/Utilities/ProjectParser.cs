using BeatSaberModdingTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BeatSaberModdingTools.Utilities.XmlNames;

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
        public bool TryParseProjectFile(out ParseResults projectResults)
        {
            projectResults = null;
            if (string.IsNullOrEmpty(FilePath)) return false;
            if (!File.Exists(FilePath)) return false;
            try
            {
                Document = XDocument.Load(FilePath);
                ParseResults results = new ParseResults(Document);
                // --- To Read & Store ---
                // <PropGroup>BeatSaberDir, Some BSMT flag
                // <ItemGroup>References
                // PostBuild
                // <ItemGroup><EmbeddedResource Include=Manifest
                // <PropGroup><PostBuildEvent>
                // --- Location to Store ---
                // Definitions prop group: to insert BeatSaberDir if it doesn't exist (Has <ProjectGuid>, <RootNamespace>, <AssemblyName>
                // References item group: to work with future reference add/remover (First ItemGroup with <Reference>?)
                //    (or figure out how to do it through VS.)
                // PostBuildEvent? Probably better to do it in the csproj.user.
                Document.IterateThroughAllElements((element, depth) =>
                {
                    switch (element.Name.LocalName)
                    {
                        case BeatSaberDir:
                            results.BeatSaberDirElement = element;
                            results.BeatSaberDir = element.Value;
                            break;
                        case PropertyGroup:
                            if (results.MainPropertyGroupElement == null && depth == 1)
                                results.MainPropertyGroupElement = element;
                            break;
                        case Reference:
                            if (element.TryGetAttribute("Include", out string includeVal))
                            {
                                element.TryFindFirstChild("HintPath", out XElement hintPathElement);
                                ReferenceModel reference = new ReferenceModel(includeVal, element.Parent, hintPathElement?.Value);
                                reference.InProject = true;
                                results.References.Add(reference);
                            }
                            return false;
                        case EmbeddedResource:
                            if(string.IsNullOrEmpty(results.ManifestFilePath))
                            {
                                if (element.TryGetAttribute("Include", out string manifestPath))
                                    results.ManifestFilePath = manifestPath;
                            }
                            return false;
                        case PostBuildEvent:
                            results.PostBuildEventElement = element;
                            results.PostBuildEvent = element.Value;
                            return false;
                        case Target:
                            if (element.TryGetAttribute("Name", out string targetName))
                                results.Targets.Add(targetName, element);
                            return false;
                        case UsingTask:
                            string taskName = element.Attributes().Where(a => a.Name.LocalName == "TaskName").Select(a => a.Value).FirstOrDefault();
                            if (!string.IsNullOrEmpty(taskName) && !results.BuildTasks.ContainsKey(taskName))
                                results.BuildTasks.Add(taskName, element);
                            return false;
                        default:
                            break;
                    }
                    return true;
                });
                projectResults = results;
                return true;
            }
            catch(Exception ex)
            {
                projectResults = new ParseResults() { Exception = ex };
                return false;
            }
        }
    }

    public class ParseResults
    {
        public XDocument Document { get; private set; }
        public XElement MainPropertyGroupElement { get; set; }
        public XElement PostBuildEventElement { get; set; }
        public Dictionary<string, XElement> Targets { get; private set; }
        public Dictionary<string, XElement> BuildTasks { get; private set; }
        public List<ReferenceModel> References { get; private set; }
        public XElement BeatSaberDirElement { get; set; }
        public string BeatSaberDir { get; set; }
        public string ManifestFilePath { get; set; }
        public string PostBuildEvent { get; set; }
        public Exception Exception { get; set; }
        public ParseResults()
        {
            Targets = new Dictionary<string, XElement>();
            BuildTasks = new Dictionary<string, XElement>();
            References = new List<ReferenceModel>();
        }
        public ParseResults(XDocument document)
        {
            Document = document;
            Targets = new Dictionary<string, XElement>();
            BuildTasks = new Dictionary<string, XElement>();
            References = new List<ReferenceModel>();
        }
    }
}
