using BeatSaberModdingTools.Models;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace BeatSaberModdingTools
{
    public class EnvironmentMonitor
    {


        public static EnvironmentMonitor Instance { get; private set; }

        public bool? BsipaProjectInSolution;

        public ConcurrentDictionary<string, ProjectModel> Projects { get; private set; }

        public async Task InitializeAsync(AsyncPackage package)
        {
            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            DTE2 dte = (await package.GetServiceAsync(typeof(SDTE)).ConfigureAwait(false)) as DTE2;
            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterLoadProject += SolutionEvents_OnAfterLoadProject;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnBeforeUnloadProject += SolutionEvents_OnBeforeUnloadProject; ;
        }

        private void SolutionEvents_OnBeforeUnloadProject(object sender, Microsoft.VisualStudio.Shell.Events.LoadProjectEventArgs e)
        {
            var thing = e.RealHierarchy;
        }

        private void SolutionEvents_OnAfterLoadProject(object sender, Microsoft.VisualStudio.Shell.Events.LoadProjectEventArgs e)
        {

        }

        public EnvironmentMonitor()
        {
            Projects = new ConcurrentDictionary<string, ProjectModel>();
        }
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
