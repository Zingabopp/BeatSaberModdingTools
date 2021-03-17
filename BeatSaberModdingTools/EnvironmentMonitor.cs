using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
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
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenProject -= _instance.SolutionEvents_OnAfterOpenProject;
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterLoadProject -= _instance.SolutionEvents_OnAfterLoadProject;
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeUnloadProject -= _instance.SolutionEvents_OnBeforeUnloadProject;
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenSolution -= _instance.SolutionEvents_OnAfterOpenSolution;
                    Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeCloseSolution -= _instance.SolutionEvents_OnBeforeCloseSolution;
                }
                _instance = value;
            }
        }

        public bool? BsipaProjectInSolution;
        private AsyncPackage package;
        public ConcurrentDictionary<string, ProjectModel> Projects { get; } = new ConcurrentDictionary<string, ProjectModel>();


        public static async Task InitializeAsync(AsyncPackage package)
        {

            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Instance = new EnvironmentMonitor();
            Instance.package = package;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenProject += Instance.SolutionEvents_OnAfterOpenProject;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterLoadProject += Instance.SolutionEvents_OnAfterLoadProject;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeUnloadProject += Instance.SolutionEvents_OnBeforeUnloadProject;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenSolution += Instance.SolutionEvents_OnAfterOpenSolution;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeCloseSolution += Instance.SolutionEvents_OnBeforeCloseSolution;
            Instance.RefreshProjects();
        }
        #region Solution Events
        private void SolutionEvents_OnAfterOpenSolution(object sender, Microsoft.VisualStudio.Shell.Events.OpenSolutionEventArgs e)
        {
            //RefreshProjects();
        }
#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void SolutionEvents_OnAfterOpenProject(object sender, Microsoft.VisualStudio.Shell.Events.OpenProjectEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ProjectModel projModel = null;
                e.Hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object projectObj);
                if (projectObj is EnvDTE.Project project)
                {
                    var newProject = EnvUtils.GetProject(project.FullName);
                    if (newProject == null) return; // This event seems to randomly trigger if the project is closed and the csproj file is opened in the editor.
                    {
                        projModel = CreateProjectModel(newProject);
                        if (Projects.TryAdd(newProject.FullPath, projModel))
                            OnProjectLoaded(newProject, projModel);
                    }
                }
                else
                {
                    var newProjects = ProjectCollection.GlobalProjectCollection.LoadedProjects.Where(p =>
                        !Projects.ContainsKey(p.FullPath)
                        && !p.FullPath.EndsWith(".user")).ToList();
                    foreach (var item in newProjects)
                    {
                        var projectModel = CreateProjectModel(item);
                        if (Projects.TryAdd(item.FullPath, projectModel))
                            OnProjectLoaded(item, projectModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Helpers.ShowErrorAsync("Open Project", $"Error in AfterOpenProject event: {ex.Message}\n{ex}");
            }
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void SolutionEvents_OnAfterLoadProject(object sender, Microsoft.VisualStudio.Shell.Events.LoadProjectEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ProjectModel projModel = null;
                e.RealHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object projectObj);

                if (projectObj is EnvDTE.Project project)
                {
                    var interfaces = project.GetType().GetInterfaces();
                    var newProject = EnvUtils.GetProject(project.FullName);
                    if (newProject != null)
                    {
                        projModel = CreateProjectModel(newProject);
                        if (Projects.TryAdd(newProject.FullPath, projModel))
                            OnProjectLoaded(newProject, projModel);
                    }
                }
                else
                {
                    var newProjects = ProjectCollection.GlobalProjectCollection.LoadedProjects.Where(p =>
                        !Projects.ContainsKey(p.FullPath)
                        && !p.FullPath.EndsWith(".user")).ToList();
                    foreach (var item in newProjects)
                    {
                        var projectModel = CreateProjectModel(item);
                        if (Projects.TryAdd(item.FullPath, projectModel))
                            OnProjectLoaded(item, projectModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Helpers.ShowErrorAsync("After Load Project", $"Error in AfterLoadProject event: {ex.Message}\n{ex}");
            }
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void SolutionEvents_OnBeforeUnloadProject(object sender, Microsoft.VisualStudio.Shell.Events.LoadProjectEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                e.RealHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object projectObj);
                EnvDTE.Project project = (EnvDTE.Project)projectObj;
                Projects.TryRemove(project.FullName, out _);
            }
            catch (Exception ex)
            {
                _ = Helpers.ShowErrorAsync("Before Unload Project", $"Error in BeforeUnloadProject event: {ex.Message}\n{ex}");
            }
        }

        private void SolutionEvents_OnBeforeCloseSolution(object sender, EventArgs e)
        {
            BsipaProjectInSolution = null;
            Projects.Clear();
        }

        #endregion
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
                ThreadHelper.ThrowIfNotOnUIThread();
                OnProjectLoaded(project, projModel);
            }
            if (BsipaProjectInSolution == null)
                BsipaProjectInSolution = false;
        }

        public bool TryGetProject(string projectFilePath, out ProjectModel projectModel, out Microsoft.Build.Evaluation.Project project)
        {
            projectModel = null;
            project = EnvUtils.GetProject(projectFilePath);
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
            string guidStr = buildProject.GetProperty("ProjectGuid")?.EvaluatedValue;
            Guid projectGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(guidStr))
                projectGuid = new Guid(buildProject.GetProperty("ProjectGuid").EvaluatedValue);
            string projectName = buildProject.GetProperty("AssemblyName").EvaluatedValue;
            var references = buildProject.GetItems("Reference").ToList();
            bool isBsipa = buildProject.GetItems("Reference").Any(i => i.EvaluatedInclude.StartsWith("IPA.Loader"));
            var bsDirProp = buildProject.GetProperty("BeatSaberDir");
            var projCap = Models.ProjectCapabilities.None;
            ProjectOptions projOptions = ProjectOptions.None;
            if (bsDirProp != null)
            {
                projCap |= Models.ProjectCapabilities.BeatSaberDir;
                projOptions |= ProjectOptions.BeatSaberDir;
            }
            var projectModel = new ProjectModel(projectGuid, projectName, projectPath, isBsipa, projOptions, projOptions, projCap);
            return projectModel;
        }



#pragma warning disable VSTHRD100 // Avoid async void methods
        public async void OnProjectLoaded(Microsoft.Build.Evaluation.Project project, ProjectModel projectModel)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                if (projectModel.IsBSIPAProject)
                    BsipaProjectInSolution = true;
                Microsoft.Build.Evaluation.Project userProj = null;
                try
                {
                    userProj = EnvUtils.GetProject(project.FullPath + ".user");
                    if (userProj == null) return;
                }
                catch (InvalidOperationException) { return; }
                var installPath = BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath;
                var projBeatSaberDir = project.GetPropertyValue("BeatSaberDir");
                var userBeatSaberDir = userProj.GetPropertyValue("BeatSaberDir");
                if (BSMTSettingsManager.Instance.CurrentSettings.GenerateUserFileOnExisting
                    && !string.IsNullOrEmpty(BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath)
                    && projectModel.IsBSIPAProject)
                {
                    Utilities.EnvUtils.SetReferencePaths(userProj, projectModel, project, null);
                    if (!string.IsNullOrEmpty(userBeatSaberDir) &&
                        userBeatSaberDir != BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath)
                    {
                        var prop = userProj.GetProperty("BeatSaberDir");
                        string message = $"Overriding BeatSaberDir in {projectModel.ProjectName} to \n{prop?.EvaluatedValue}\n(Old path: {userBeatSaberDir})";
                        VsShellUtilities.ShowMessageBox(
                            this.package,
                            message,
                            $"{projectModel.ProjectName}: Auto Set BeatSaberDir",
                            OLEMSGICON.OLEMSGICON_INFO,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Helpers.ShowErrorAsync("OnProjectLoaded", $"Error in OnProjectLoaded: {ex.Message}\n{ex}");
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
