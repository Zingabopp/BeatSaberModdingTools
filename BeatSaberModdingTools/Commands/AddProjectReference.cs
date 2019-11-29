using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberModdingTools.Views;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using Task = System.Threading.Tasks.Task;

namespace BeatSaberModdingTools.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AddProjectReference
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0103;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6a1cb889-cf43-4fe1-9eb7-9370d0d8d1d4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddProjectReference"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AddProjectReference(AsyncPackage package, OleMenuCommandService commandService)
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
        public static AddProjectReference Instance
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
            // Switch to the main thread - the call to AddCommand in AddProjectReference's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AddProjectReference(package, commandService);
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
            SelectedItem selectedItem = dte.SelectedItems.Item(1);
            var csProj = (VSProject)selectedItem.Project.Object;
            //csProj.References.Add("path");
            //csProj.References.Item(1).Remove();
            string projectFilePath = selectedItem.Project.FullName;
            var settingsDialog = new ReferencesDialog(projectFilePath);

            var returnedTrue = settingsDialog.ShowDialog() ?? false;
            var changedRefs = settingsDialog.ViewModel.AvailableReferences.Where(r => r.StartedInProject != r.IsInProject).ToList();
            var removedRefs = changedRefs.Where(r => !r.IsInProject).ToList();
            foreach (var item in removedRefs)
            {
                var reference = csProj.References.Find(item.Name);
                reference.Remove();
            }
            var addedRefs = changedRefs.Where(r => r.IsInProject).ToList();
            foreach (var item in addedRefs)
            {
                var refPath = item.HintPath.Replace(settingsDialog.ViewModel.BeatSaberDir, "$(BeatSaberDir)");
                var reference = csProj.References.Add(item.HintPath);
                reference.CopyLocal = false;
                
            }
            var buildProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(csProj.Project.FullName).First();
            foreach (var item in addedRefs)
            {
                var needsHint = buildProject.Items.Where(obj => obj.ItemType == "Reference" && obj.EvaluatedInclude == item.Name).First();
                needsHint.SetMetadataValue("HintPath", $"$(BeatSaberDir)\\{item.RelativeDirectory}\\{item.Name}.dll");
            }
            csProj.Project.Save();

            //if (returnedTrue)
            //    BSMTSettingsManager.Instance.Store(settingsDialog.ReturnSettings);
        }
    }
}
