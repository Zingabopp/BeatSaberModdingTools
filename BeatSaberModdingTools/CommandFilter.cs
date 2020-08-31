using BeatSaberModdingTools.Commands;
using BeatSaberModdingTools.Menus;
using BeatSaberModdingTools.Models;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System;
using static BeatSaberModdingTools.Utilities.EnvUtils;

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
            uint cmdId = prgCmds[0].cmdID;
            bool visible = false;
            bool available = false;
            bool enabled = false;
            CommandState status;
            if (pguidCmdGroup.Equals(CommandSetGuids.ReferencesContextCmdSet))
            {
                if (cmdId == AddProjectReference.CommandId)
                {
                    if (TryGetSelectedProject(package, out ProjectModel proj) && proj.IsBSIPAProject)
                    {
                        available = true;
                        enabled = true;
                        visible = true;
                    }
                }
            }
            else if (pguidCmdGroup.Equals(CommandSetGuids.ProjectContextCmdSet))
            {
                if (cmdId == ProjectContextSubmenu.CommandId)
                {
                    if (TryGetSelectedProject(package, out ProjectModel proj) && proj.IsBSIPAProject)
                    {
                        available = true;
                        enabled = true;
                        visible = true;
                    }
                }
                else if (cmdId == SetBeatSaberDirCommand.CommandId)
                {
                    if (TryGetSelectedProject(package, out ProjectModel projectModel, out Project project) && projectModel.IsBSIPAProject)
                    {
                        available = true;
                        string prop = project.GetProperty("BeatSaberDir")?.UnevaluatedValue;
                        if (!string.IsNullOrEmpty(prop))
                            enabled = true;
                        visible = true;
                    }
                }
                else if (cmdId == AddProjectReferencePaths.CommandId)
                {
                    if (TryGetSelectedProject(package, out ProjectModel projectModel, out Project _)
                        && projectModel.IsBSIPAProject)
                    {
                        available = true;
                        enabled = true;
                        visible = true;
                    }
                }
            }
            status = new CommandState(available, false, enabled, visible);
            prgCmds[0].cmdf = (uint)GetVsStatus(status);
            return VSConstants.S_OK;
        }


        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return NextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private OLECMDF GetVsStatus(CommandState commandStatus)
        {
            OLECMDF ret = 0;
            if (commandStatus.IsAvailable)
                ret |= OLECMDF.OLECMDF_SUPPORTED;
            if (commandStatus.IsEnabled)
                ret |= OLECMDF.OLECMDF_ENABLED;
            if (!commandStatus.IsVisible)
                ret |= OLECMDF.OLECMDF_INVISIBLE;
            return ret;
        }
    }
}
