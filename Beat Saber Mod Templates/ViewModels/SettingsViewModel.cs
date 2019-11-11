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
        public SettingsViewModel()
        {
            var detectedLocations = BeatSaberLocator.GetBeatSaberPathsFromRegistry();
            BeatSaberLocations = new ObservableCollection<BeatSaberInstall>(detectedLocations);
            AddLocation(new BeatSaberInstall(@"C:\TestPath", InstallType.Oculus));
        }
        public ObservableCollection<BeatSaberInstall> BeatSaberLocations { get; set; }

        public bool AddLocation(BeatSaberInstall beatSaberInstall)
        {
            if(string.IsNullOrEmpty(beatSaberInstall.Path))
            {
                return false;
            }
            var fullInstallPath = Path.GetFullPath(beatSaberInstall.Path);
            if (BeatSaberLocations.Any(i => string.Equals(fullInstallPath, Path.GetFullPath(i.Path), StringComparison.CurrentCultureIgnoreCase)))
            {
                BeatSaberLocations.Add(beatSaberInstall);
                return true;
            }
            return false;
        }
    }
}
