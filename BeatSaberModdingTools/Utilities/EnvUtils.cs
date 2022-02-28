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
            ThreadHelper.ThrowIfNotOnUIThread();
            return TryGetSelectedProject(package, out projectModel, out _, out _);
        }

        public static bool TryGetSelectedProject(AsyncPackage package, out ProjectModel projectModel, out Microsoft.Build.Evaluation.Project project, out EnvDTE.Project dteProject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            projectModel = null;
            project = null;
            dteProject = null;
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
            dteProject = (EnvDTE.Project)activeProjects.GetValue(0);
            return EnvironmentMonitor.Instance.TryGetProject(dteProject.FullName, out projectModel, out project);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userProj"></param>
        /// <param name="projectModel"></param>
        /// <param name="project"></param>
        /// <param name="dteProject"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string SetReferencePaths(Project userProj, ProjectModel projectModel, Project project, EnvDTE.Project dteProject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string beatSaberDir = BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath;
            string userProjPath = userProj?.FullPath ?? project.FullPath + ".user";
            userProj = userProj ?? ProjectCollection.GlobalProjectCollection.GetLoadedProjects(userProjPath).FirstOrDefault();
            bool userFileCreated = false;
            if (userProj == null)
            {
                if (!File.Exists(userProjPath))
                {
                    File.WriteAllText(userProjPath, CreateUserProject(beatSaberDir));
                    userProj = ProjectCollection.GlobalProjectCollection.LoadProject(userProjPath);
                    userFileCreated = true;
                    //dteProject.ProjectItems.AddFromFile(userProjPath);
                }
                if (File.Exists(userProjPath))
                    userProj = ProjectCollection.GlobalProjectCollection.LoadProject(userProjPath);
            }
            
            string hintPathsStr = GetReferencePathString(beatSaberDir) ?? throw new ArgumentException("Error setting ReferencePath, chosen install path is null or empty.");
            ProjectProperty prop = userProj?.SetProperty("ReferencePath", hintPathsStr) ?? throw new InvalidOperationException("Could not access or create csproj.user file.");
            userProj.MarkDirty();
            userProj.Save();
            project.MarkDirty();
            project.ReevaluateIfNecessary();
            return $"Setting ReferencePath in {projectModel.ProjectName} to \n{prop.EvaluatedValue}{(userFileCreated ? "\n\nYou may need to reload the project." : "")}";
        }

        public static string SetBeatSaberDir(Project userProj, ProjectModel projectModel, Project project, EnvDTE.Project dteProject)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string beatSaberDir = null;
            string userProjPath = null;
            bool userFileCreated = false;
            try
            {
                beatSaberDir = BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath;
                userProjPath = userProj?.FullPath ?? project.FullPath + ".user";
                userProj = userProj ?? ProjectCollection.GlobalProjectCollection.GetLoadedProjects(userProjPath).FirstOrDefault();
                if (userProj == null)
                {
                    if (!File.Exists(userProjPath))
                    {
                        File.WriteAllText(userProjPath, CreateUserProject(beatSaberDir));
                        userProj = ProjectCollection.GlobalProjectCollection.LoadProject(userProjPath);
                        userFileCreated = true;
                        //dteProject.ProjectItems.AddFromFile(userProjPath);
                    }
                    if (File.Exists(userProjPath))
                        userProj = ProjectCollection.GlobalProjectCollection.LoadProject(userProjPath);
                }
                ProjectProperty prop = userProj?.SetProperty("BeatSaberDir", beatSaberDir) ?? throw new InvalidOperationException("Could not access or create csproj.user file.");
                userProj.MarkDirty();
                userProj.Save();
                project.MarkDirty();
                project.ReevaluateIfNecessary(); 
                return $"Setting BeatSaberDir in {projectModel.ProjectName} to \n{prop.EvaluatedValue}{(userFileCreated ? "\n\nYou may need to reload the project." : "")}";

            }
            catch (Exception ex)
            {
                return $"Failed to set BeatSaberDir: {ex.Message}\nbeatSaberDir: '{beatSaberDir}'|userProjLoaded: {userProj != null}|userProjPath: '{userProjPath}'\n{ex.StackTrace}";
            }
        }

        public static string GetReferencePathString(string beatSaberDir)
        {
            if (string.IsNullOrWhiteSpace(beatSaberDir))
                return null;
            string[] referencePaths = new string[] { Path_Managed, Path_Libs, Path_Plugins }.Select(p => Path.Combine(beatSaberDir, p)).ToArray();
            return string.Join(";", referencePaths);
        }

        public static Microsoft.Build.Evaluation.Project GetProject(string fullPath)
        {
            bool isValid = false;
            bool isUser = false;
            if (fullPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                isValid = true;
            else if (fullPath.EndsWith(".csproj.user", StringComparison.OrdinalIgnoreCase))
            {
                isValid = true;
                isUser = true;
            }
            else
                return null;
            Project project = ProjectCollection.GlobalProjectCollection.LoadedProjects.Where(p => p.FullPath == fullPath).FirstOrDefault();
            if (project == null && File.Exists(fullPath))
            {
                project = ProjectCollection.GlobalProjectCollection.LoadProject(fullPath);
                string userProjectName = fullPath + ".user";
                if (!isUser && File.Exists(userProjectName))
                    ProjectCollection.GlobalProjectCollection.LoadProject(fullPath + ".user");
                return project;
            }
            return ProjectCollection.GlobalProjectCollection.LoadedProjects.Where(p => p.FullPath == fullPath).SingleOrDefault();
        }

        public static void SaveProject(Project project)
        {
            if (File.Exists(project.FullPath))
                project.Save();
        }

        public static string CreateUserProject(string beatSaberDir = null)
        {
            string xml;
            if (!string.IsNullOrWhiteSpace(beatSaberDir))
                xml = UserProjectTemplate.Replace("<BeatSaberDir>", $"<BeatSaberDir>{beatSaberDir}");
            else
                xml = UserProjectTemplate.Replace("<BeatSaberDir></BeatSaberDir>", "<!--<BeatSaberDir></BeatSaberDir>-->");
            return xml;
        }
        public static readonly string UserProjectTemplate = "<?xml version =\"1.0\" encoding=\"utf-8\"?>\r\n<Project>\r\n  <PropertyGroup>\r\n    <BeatSaberDir></BeatSaberDir>\r\n  </PropertyGroup>\r\n</Project>";
    }
}
