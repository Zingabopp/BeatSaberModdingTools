using BeatSaberModTemplates.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<BeatSaberInstall> DesignExample => new ObservableCollection<BeatSaberInstall>()
        {
            new BeatSaberInstall(@"C:\SteamInstall", InstallType.Steam),
            new BeatSaberInstall(@"C:\OculusInstall\DDDDDDDDDD\AAAAAAAAAA\VVVVVVVVVVVV\CCCCCCCCCCCCCC\SSSSSSSSSSSSSS\F", InstallType.Oculus),
            new BeatSaberInstall(@"C:\ManualInstall", InstallType.Manual)
        };

        public SettingsViewModel()
        {
            var detectedLocations = BeatSaberLocator.GetBeatSaberPathsFromRegistry();
            BeatSaberLocations = new ObservableCollection<BeatSaberInstall>(detectedLocations);
            AddLocation(new BeatSaberInstall(@"C:\OculusInstall\DDDDDDDDDD\AAAAAAAAAA\VVVVVVVVVVVV\CCCCCCCCCCCCCC\SSSSSSSSSSSSSS\F", InstallType.Oculus));
            AddLocation(new BeatSaberInstall(@"C:\ManualInstall", InstallType.Manual));
        }

        /// <summary>
        /// Beat Saber install locations
        /// </summary>
        public ObservableCollection<BeatSaberInstall> BeatSaberLocations { get; set; }

        /// <summary>
        /// If true, automatically generate a csproj.user file with the chosen BeatSaberDir when creating projects from supported templates.
        /// </summary>
        public bool GenerateUserFileWithTemplate { get; set; }

        /// <summary>
        /// If true, generate a csproj.user file when an existing project is opened that contains a BeatSaberDir property.
        /// </summary>
        public bool GenerateUserFileOnExisting { get; set; }

        private BeatSaberInstall _chosenInstall;
        /// <summary>
        /// The currently used install location.
        /// </summary>
        public BeatSaberInstall ChosenInstall
        {
            get { return _chosenInstall; }
            set
            {
                if (_chosenInstall == null && value == null)
                    return;
                throw new NotImplementedException();
            }
        }

        public bool AddLocation(BeatSaberInstall beatSaberInstall)
        {
            if (string.IsNullOrEmpty(beatSaberInstall.Path))
            {
                return false;
            }
            var fullInstallPath = Path.GetFullPath(beatSaberInstall.Path);
            if (!BeatSaberLocations.Any(i => string.Equals(fullInstallPath, Path.GetFullPath(i.Path), StringComparison.CurrentCultureIgnoreCase)))
            {
                BeatSaberLocations.Add(beatSaberInstall);
                return true;
            }
            return false;
        }
    }
}
