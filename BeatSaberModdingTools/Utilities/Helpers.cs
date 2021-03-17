using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace BeatSaberModdingTools.Utilities
{
    public static class Helpers
    {
        public static AsyncPackage Package { get; set; }
        public static async Task ShowErrorAsync(string title, string message)
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                _ = VsShellUtilities.ShowMessageBox(
                        Package,
                        message,
                        $"BSMT | {title}",
                        icon,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
            catch (Exception)
            {

            }
        }
    }
}
