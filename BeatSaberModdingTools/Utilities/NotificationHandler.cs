using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace BeatSaberModdingTools.Utilities
{
    public class NotificationHandler : INotificationHandler
    {
        public AsyncPackage Package { get; set; }

        public NotificationHandler(AsyncPackage package)
        {
            Package = package;
        }

        public void ShowError(string title, string message)
        {
            if(ThreadHelper.CheckAccess())
            {
                OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                _ = VsShellUtilities.ShowMessageBox(
                        Package,
                        message,
                        $"BSMT | {title}",
                        icon,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
            else
            {
                Package.JoinableTaskFactory.Run(() =>
                {
                    OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    _ = VsShellUtilities.ShowMessageBox(
                            Package,
                            message,
                            $"BSMT | {title}",
                            icon,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return Task.CompletedTask;
                });
            }
        }

        public async Task ShowErrorAsync(string title, string message)
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ShowError(title, message);
            }
            catch (Exception)
            {

            }
        }

    }
}
