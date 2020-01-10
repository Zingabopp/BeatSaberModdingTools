using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;
using static BeatSaberModdingTools.Utilities.EnvUtils;
using Microsoft.Build.Evaluation;
using static BeatSaberModdingTools.Utilities.Paths;
using BeatSaberModdingTools.Models;

namespace BeatSaberModdingTools.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AddProjectReferencePaths
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0102;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6a1cb889-cf43-4fe1-9eb7-9370d0d8d1d5");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddProjectReferencePaths"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AddProjectReferencePaths(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandID = new CommandID(CommandSet, CommandId);
            //var menuItem = new MenuCommand(this.Execute, menuCommandID);
            var menuItem = new OleMenuCommand(Execute, menuCommandID, true);
            commandService.AddCommand(menuItem);
        }


        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AddProjectReferencePaths Instance
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
            // Switch to the main thread - the call to AddCommand in AddProjectReferencePaths's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AddProjectReferencePaths(package, commandService);

        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string message;
            string title = "Error adding reference paths:";
            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
            if (TryGetSelectedProject(package, out ProjectModel projectModel, out var project))
            {
                var userProj = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectModel.ProjectPath + ".user").FirstOrDefault();
                if (userProj != null)
                {
                    message = SetReferencePaths(userProj, projectModel, project);
                    icon = OLEMSGICON.OLEMSGICON_INFO;
                }
                else
                    message = "Unable to retrieve user specific project.";
            }
            else
                message = "Unable to retrieve project information.";
            // OK = 1, Cancel = 2, Abort = 3, Retry = 4, Ignore = 5, Yes = 6, No = 7 depending on what button is pressed.
            int result = VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                icon,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
