using BeatSaberModdingTools.Models;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLangProj;
using Task = System.Threading.Tasks.Task;

namespace BeatSaberModdingTools
{
    public class EnvironmentMonitor
    {
        private static EnvironmentMonitor _instance;
        public static EnvironmentMonitor Instance
        {
            get { return _instance; }
            private set
            {
                if (_instance == value) return;
                if (_instance != null)
                {
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterLoadProject -= _instance.SolutionEvents_OnAfterLoadProject;
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeUnloadProject -= _instance.SolutionEvents_OnBeforeUnloadProject;
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenSolution -= _instance.SolutionEvents_OnAfterOpenSolution;
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeCloseSolution -= _instance.SolutionEvents_OnBeforeCloseSolution;
                }
                _instance = value;
            }
        }

        public bool? BsipaProjectInSolution;

        public ConcurrentDictionary<string, ProjectModel> Projects { get; } = new ConcurrentDictionary<string, ProjectModel>();


        public static async Task InitializeAsync(AsyncPackage package)
        {
            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Instance = new EnvironmentMonitor();

            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterLoadProject += Instance.SolutionEvents_OnAfterLoadProject;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeUnloadProject += Instance.SolutionEvents_OnBeforeUnloadProject;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenSolution += Instance.SolutionEvents_OnAfterOpenSolution;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeCloseSolution += Instance.SolutionEvents_OnBeforeCloseSolution;
            Instance.RefreshProjects();
        }

        private void CreateCmd_AfterExecute(string Guid, int ID, object CustomIn, object CustomOut)
        {
            if (Guid == "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}")
            {
                if (ID == 1627 || ID == 1990)
                    return;
            }
            if (Guid == "{5EFC7975-14BC-11CF-9B2B-00AA00573819}")
            {
                if (ID == 684 || ID == 337)
                    return;
            }
            var thing = Guid;
        }

        private void SolutionEvents_OnBeforeCloseSolution(object sender, EventArgs e)
        {
            Projects.Clear();
        }

        public void RefreshProjects()
        {
            BsipaProjectInSolution = null;
            Projects.Clear();
            var loadedProjects = ProjectCollection.GlobalProjectCollection.LoadedProjects.Where(p => p.ProjectFileLocation.File.EndsWith(".csproj")).ToList();
            foreach (var project in loadedProjects)
            {
                var projModel = CreateProjectModel(project);
                if (projModel.IsBSIPAProject)
                    BsipaProjectInSolution = true;
                Projects.TryAdd(projModel.ProjectPath, projModel);
            }
            if (BsipaProjectInSolution == null)
                BsipaProjectInSolution = false;
        }

        public bool TryGetProject(string projectFilePath, out ProjectModel projectModel, out Microsoft.Build.Evaluation.Project project)
        {
            projectModel = null;
            project = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectFilePath).FirstOrDefault();
            bool retVal = false;
            if (Projects.TryGetValue(projectFilePath, out projectModel))
                retVal = true;
            else if (project != null)
            {
                projectModel = CreateProjectModel(project);
                Projects.TryAdd(projectFilePath, projectModel);
                if (projectModel.IsBSIPAProject)
                    BsipaProjectInSolution = true;
                retVal = true;
            }
            return retVal;
        }

        public ProjectModel CreateProjectModel(Microsoft.Build.Evaluation.Project buildProject)
        {
            string projectPath = buildProject.ProjectFileLocation.File;

            Guid projectGuid = new Guid(buildProject.GetProperty("ProjectGuid").EvaluatedValue);
            string projectName = buildProject.GetProperty("AssemblyName").EvaluatedValue;
            var references = buildProject.GetItems("Reference").ToList();
            bool isBsipa = buildProject.GetItems("Reference").Any(i => i.EvaluatedInclude.StartsWith("IPA.Loader"));
            var bsDirProp = buildProject.GetProperty("BeatSaberDir");
            ProjectCapabilities projCap = ProjectCapabilities.None;
            ProjectOptions projOptions = ProjectOptions.None;
            if (bsDirProp != null)
            {
                projCap |= ProjectCapabilities.BeatSaberDir;
                projOptions |= ProjectOptions.BeatSaberDir;
            }
            var projectModel = new ProjectModel(projectGuid, projectName, projectPath, isBsipa, projOptions, projOptions, projCap);
            return projectModel;
        }

        private void SolutionEvents_OnAfterOpenSolution(object sender, Microsoft.VisualStudio.Shell.Events.OpenSolutionEventArgs e)
        {
            RefreshProjects();
        }

        private void SolutionEvents_OnBeforeUnloadProject(object sender, Microsoft.VisualStudio.Shell.Events.LoadProjectEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            e.RealHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object projectObj);
            EnvDTE.Project project = (EnvDTE.Project)projectObj;
            Projects.TryRemove(project.FullName, out _);
        }

        private void SolutionEvents_OnAfterLoadProject(object sender, Microsoft.VisualStudio.Shell.Events.LoadProjectEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ProjectModel projModel = null;
            e.RealHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object projectObj);
            if (projectObj is EnvDTE.Project project)
            {
                var newProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(project.FullName).First();
                projModel = CreateProjectModel(newProject);

            }
            else
            {
                var newProjects = ProjectCollection.GlobalProjectCollection.LoadedProjects.Where(p =>
                    !Projects.ContainsKey(p.FullPath)
                    && !p.FullPath.EndsWith(".user")).ToList();
                foreach (var item in newProjects)
                {
                    Projects.TryAdd(item.FullPath, CreateProjectModel(item));
                }
            }

        }

        public EnvironmentMonitor()
        { }

        public event EventHandler<BsipaProjectLoadedEventArgs> BsipaProjectLoaded;
    }

    public class BsipaProjectLoadedEventArgs
    {
        public string ProjectPath { get; protected set; }
        public BsipaProjectLoadedEventArgs(string projectPath)
        {
            ProjectPath = projectPath;
        }
    }
}
