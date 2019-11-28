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
        public static readonly Guid CommandSet = new Guid("6a1cb889-cf43-4fe1-9eb7-9370d0d8d1d4");

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
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            bool commandVisibleAndEnabled = false;
            if (menuCommand != null)
            {
                if (Directory.Exists(BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath))
                    commandVisibleAndEnabled = true;
                menuCommand.Enabled = commandVisibleAndEnabled;
                menuCommand.Visible = commandVisibleAndEnabled;
            }

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
            DTE2 dte = (await package.GetServiceAsync(typeof(SDTE)).ConfigureAwait(false)) as DTE2;
            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (dte.SelectedItems.Count != 1) return;
            var selectedList = new List<string>();
            var selectedItem = dte.SelectedItems.Item(1);
            selectedList.Add(selectedItem.Project.FullName);
            selectedList.Add($"---{BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath}---\n");
            foreach(EnvDTE.Property item in selectedItem.Project.Properties)
            {
                selectedList.Add(item.Name);
            }
            string message = string.Format(CultureInfo.CurrentCulture, "SelectedItems: {0}", string.Join(", ", selectedList));
            string title = "AddProjectReferencePaths";
            // OK = 1, Cancel = 2, Abort = 3, Retry = 4, Ignore = 5, Yes = 6, No = 7 depending on what button is pressed.
            int result = VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            //dte.ExecuteCommand("Project.UnloadProject");
            //dte.ExecuteCommand("Project.ReloadProject");
        }
    }
}
