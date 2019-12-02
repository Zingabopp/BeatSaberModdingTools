using BeatSaberModdingTools.Models;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
