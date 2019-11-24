using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Task = System.Threading.Tasks.Task;

namespace BeatSaberModdingTools.Menus
{
    public class ProjectContextSubmenu
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6a1cb889-cf43-4fe1-9eb7-9370d0d8d1d4");  // get the GUID from the .vsct file

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public const int CommandId = 0x1021;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ProjectContextSubmenu Instance
        {
            get;
            private set;
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
            Instance = new ProjectContextSubmenu(package, commandService);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectContextSubmenu"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ProjectContextSubmenu(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(null, menuCommandID, true);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }
        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            bool commandVisibleAndEnabled = true;
            if (menuCommand != null)
            {
                menuCommand.Enabled = commandVisibleAndEnabled;
                menuCommand.Visible = commandVisibleAndEnabled;
            }

        }
    }
}
