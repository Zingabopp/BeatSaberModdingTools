using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Models
{
    public class ProjectModel
    {
        public Guid ProjectGuid { get; private set; }
        public string ProjectName { get; private set; }
        public string ProjectPath { get; private set; }
        public bool IsBSIPAProject { get; private set; }
        public ProjectOptions ProjectOptions { get; private set; }
        public ProjectOptions UserFileOptions { get; private set; }
        public ProjectCapabilities SupportedCapabilities { get; private set; }

        public ProjectModel(Guid projectGuid, string projectName, string projectPath, bool isBsipaProject,
            ProjectOptions projectOptions = ProjectOptions.None, ProjectOptions userFileOptions = ProjectOptions.None,
            ProjectCapabilities supportedCapabilities = ProjectCapabilities.None)
        {
            ProjectGuid = projectGuid;
            ProjectName = projectName;
            ProjectPath = projectPath;
            IsBSIPAProject = isBsipaProject;
            ProjectOptions = projectOptions;
            UserFileOptions = userFileOptions;
            SupportedCapabilities = supportedCapabilities;
        }
    }

    [Flags]
    public enum ProjectOptions
    {
        None = 0,
        BeatSaberDir = 1 << 0,
        BuildTools = 1 << 1
    }

    [Flags]
    public enum ProjectCapabilities
    {
        None = 0,
        BeatSaberDir = 1 << 0,
        DirectoryJunctions = 1 << 1,
        BuildTools = 1 << 2,
        ReferencePaths = 1 << 3
    }
}
