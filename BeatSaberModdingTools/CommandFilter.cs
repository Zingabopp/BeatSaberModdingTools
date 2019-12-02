using BeatSaberModdingTools.Commands;
using BeatSaberModdingTools.Menus;
using BeatSaberModdingTools.Models;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace BeatSaberModdingTools
{
    // From https://stackoverflow.com/a/57166903
    public class CommandFilter : IOleCommandTarget
    {
        private AsyncPackage package;
        public CommandFilter(IOleCommandTarget nextTarget, AsyncPackage asyncPackage)
        {
            NextTarget = nextTarget;
            package = asyncPackage;
        }
        
        public IOleCommandTarget NextTarget { get; set; }
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var cmdId = prgCmds[0].cmdID;
            if (pguidCmdGroup.Equals(CommandSetGuids.ReferencesContextCmdSet))
            {
                if (cmdId == AddProjectReference.CommandId)
                {
                    bool visible = false;
                    ProjectModel proj = null;
                    CommandStatus status = 0;
                    if (TryGetSelectedProject(out proj) && proj.IsBSIPAProject)
                    {
                        status |= CommandStatus.Supported;
                        status |= CommandStatus.Enabled;
                        visible = true;
                    }
                    if (visible)
                        status &= ~CommandStatus.Invisible;
                    else
                        status |= CommandStatus.Invisible;
                    prgCmds[0].cmdf = (uint)GetVsStatus(status);
                }
            }
            if (pguidCmdGroup.Equals(CommandSetGuids.ProjectContextCmdSet))
            {
                if (cmdId == ProjectContextSubmenu.CommandId)
                {
                    bool visible = false;
                    ProjectModel proj = null;
                    CommandStatus status = 0;
                    if (TryGetSelectedProject(out proj) && proj.IsBSIPAProject)
                    {
                        status |= CommandStatus.Supported;
                        status |= CommandStatus.Enabled;
                        visible = true;
                    }
                    if (visible)
                        status &= ~CommandStatus.Invisible;
                    else
                        status |= CommandStatus.Invisible;
                    prgCmds[0].cmdf = (uint)GetVsStatus(status);
                }
            }


            return VSConstants.S_OK;
        }

        private WeakReference<DTE2> _dte = new WeakReference<DTE2>(null);

        public bool TryGetSelectedProject(out ProjectModel projectModel)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            projectModel = null;
            DTE2 dte = null;
            if (!_dte.TryGetTarget(out dte))
            {
                var serviceContainer = (IServiceContainer)package;
                dte = serviceContainer.GetService(typeof(SDTE)) as DTE2;
                _dte.SetTarget(dte);
            }
            if (dte.SelectedItems.Count != 1) return false;
            Array activeProjects = (Array)dte.ActiveSolutionProjects;
            if (activeProjects.Length != 1) return false;
            var proj = (EnvDTE.Project)activeProjects.GetValue(0);
            return EnvironmentMonitor.Instance.Projects.TryGetValue(proj.FullName, out projectModel);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return NextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private OLECMDF GetVsStatus(CommandStatus commandStatus)
        {
            OLECMDF ret = 0;
            if (commandStatus.HasFlag(CommandStatus.Supported))
                ret |= OLECMDF.OLECMDF_SUPPORTED;
            if (commandStatus.HasFlag(CommandStatus.Enabled))
                ret |= OLECMDF.OLECMDF_ENABLED;
            if (commandStatus.HasFlag(CommandStatus.Invisible))
                ret |= OLECMDF.OLECMDF_INVISIBLE;
            return ret;
        }
    }
}
