using BeatSaberModdingTools.Models;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BeatSaberModdingTools.Utilities.Paths;

namespace BeatSaberModdingTools.Utilities
{
    public static class EnvUtils
    {

        private static readonly WeakReference<DTE2> _dte = new WeakReference<DTE2>(null);
        public static bool TryGetSelectedProject(AsyncPackage package, out ProjectModel projectModel)
        {
            return TryGetSelectedProject(package, out projectModel, out _);
        }

        public static bool TryGetSelectedProject(AsyncPackage package, out ProjectModel projectModel, out Microsoft.Build.Evaluation.Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            projectModel = null;
            project = null;
            if (!_dte.TryGetTarget(out DTE2 dte))
            {
                var serviceContainer = (IServiceContainer)package;
                object dteObj = serviceContainer.GetService(typeof(SDTE));
                if (dteObj == null) return false;
                dte = dteObj as DTE2;
                _dte.SetTarget(dte);
            }
            if (dte.SelectedItems.Count != 1) return false;
            Array activeProjects = (Array)dte.ActiveSolutionProjects;
            if (activeProjects.Length != 1) return false;
            var proj = (EnvDTE.Project)activeProjects.GetValue(0);
            return EnvironmentMonitor.Instance.TryGetProject(proj.FullName, out projectModel, out project);
        }

        public static string SetReferencePaths(Project userProj, ProjectModel projectModel, Project project)
        {
            string beatSaberDir = BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath;
            ProjectProperty prop = userProj.SetProperty("BeatSaberDir", beatSaberDir);
            string[] referencePaths = new string[] { Path_Managed, Path_Libs, Path_Plugins }.Select(p => Path.Combine(beatSaberDir, p)).ToArray();
            string hintPathsStr = string.Join(";", referencePaths);
            userProj.SetProperty("ReferencePath", hintPathsStr);
            userProj.Save();
            project.MarkDirty();
            return $"Setting BeatSaberDir in {projectModel.ProjectName} to \n{prop.EvaluatedValue}";
        }
    }
}
