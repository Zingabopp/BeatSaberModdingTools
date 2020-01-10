using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using static BeatSaberModdingTools.Utilities.EnvUtils;
using static BeatSaberModdingTools.Utilities.Paths;
using Microsoft.Build.Evaluation;
using System.Linq;
using System.IO;

namespace BeatSaberModdingTools.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SetBeatSaberDirCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0101;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6a1cb889-cf43-4fe1-9eb7-9370d0d8d1d5");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetBeatSaberDirCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SetBeatSaberDirCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SetBeatSaberDirCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in SetBeatSaberDirCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SetBeatSaberDirCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string title = "Set BeatSaberDir";
            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
            string message;
            if(string.IsNullOrEmpty(BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath))
            {
                icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                message = "You don't appear to have a Beat Saber install path chosen in 'Extensions > Beat Saber Modding Tools > Settings'.";
            }
            else if (TryGetSelectedProject(package, out ProjectModel projectModel, out Project project))
            {
                if (projectModel.IsBSIPAProject)
                {
                    if (projectModel.SupportedCapabilities.HasFlag(ProjectCapabilities.BeatSaberDir))
                    {
                        var userProj = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectModel.ProjectPath + ".user").FirstOrDefault();
                        if (userProj != null)
                        {
                            message = SetReferencePaths(userProj, projectModel, project);
                            icon = OLEMSGICON.OLEMSGICON_INFO;
                        }
                        else
                            message = "Unable to find .user project (this shouldn't happen).";
                    }
                    else
                        message = $"Project {projectModel.ProjectName} doesn't support the BeatSaberDir property";
                }
                else
                    message = $"Project {projectModel.ProjectName} does not appear to be a BSIPA project.";
            }
            else
                message = "Unable to get project information.";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                icon,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
