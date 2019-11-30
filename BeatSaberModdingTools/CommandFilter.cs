using BeatSaberModdingTools.Commands;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.ProjectSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools
{
    // From https://stackoverflow.com/a/57166903
    public class CommandFilter : IOleCommandTarget
    {
        public CommandFilter(IOleCommandTarget nextTarget)
        {
            NextTarget = nextTarget;
        }
        bool visible = false;
        public IOleCommandTarget NextTarget { get; set; }
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            var cmdId = prgCmds[0].cmdID;
            if (pguidCmdGroup.Equals(AddProjectReference.CommandSet) && cmdId == AddProjectReference.CommandId)
            {
                CommandStatus status = 0;
                status |= CommandStatus.Supported;
                status |= CommandStatus.Enabled;
                if (visible)
                    status &= ~CommandStatus.Invisible;
                else
                    status |= CommandStatus.Invisible;
                visible = !visible;
                prgCmds[0].cmdf = (uint)GetVsStatus(status);
            }
            return VSConstants.S_OK;
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
