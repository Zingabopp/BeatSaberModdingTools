using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using BeatSaberModdingTools.Commands;
using Task = System.Threading.Tasks.Task;
using BeatSaberModdingTools.Models;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Events;

namespace BeatSaberModdingTools
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(BeatSaberModdingToolsPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [InstalledProductRegistration("Beat Saber Modding Tools", "Provides tools and templates for creating Beat Saber mods and Visual Studio commands for managing references.", "1.0")]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class BeatSaberModdingToolsPackage : AsyncPackage
    {
        /// <summary>
        /// BeatSaberModdingToolsPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "ce163df3-bae3-4fe0-882b-da2bde0f5d8e";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            BSMTSettingsManager.SetManager(new BSMTSettingsManager());

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(false, cancellationToken);
            await BeatSaberModdingTools.Commands.AddProjectReferencePaths.InitializeAsync(this);
            await SetBeatSaberDirCommand.InitializeAsync(this);
            await BeatSaberModdingTools.Commands.OpenSettingsWindowCommand.InitializeAsync(this);
            await BeatSaberModdingTools.Menus.ProjectContextSubmenu.InitializeAsync(this);
            await BeatSaberModdingTools.Commands.AddProjectReference.InitializeAsync(this);
        }

        #endregion
    }
}
