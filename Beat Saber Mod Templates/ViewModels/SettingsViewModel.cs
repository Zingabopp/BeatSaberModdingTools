using BeatSaberModTemplates.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BeatSaberModTemplates.Utilities;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.Windows.Threading;

namespace BeatSaberModTemplates.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly SynchronizationContext _syncContext;
        public ObservableCollection<BeatSaberInstall> DesignExample => new ObservableCollection<BeatSaberInstall>()
        {
            new BeatSaberInstall(@"C:\SteamInstall", InstallType.Steam),
            new BeatSaberInstall(@"C:\OculusInstall\DDDDDDDDDD\AAAAAAAAAA\VVVVVVVVVVVV\CCCCCCCCCCCCCC\SSSSSSSSSSSSSS\F", InstallType.Oculus),
            new BeatSaberInstall(@"C:\ManualInstall", InstallType.Manual)
        };

        public SettingsViewModel()
        {
            _syncContext = SynchronizationContext.Current;
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

        private RelayCommand<string> _addInstall;
        public RelayCommand<string> AddInstall
        {
            get
            {
                if (_addInstall == null)
                    _addInstall = new RelayCommand<string>(s =>
                    {
                        AddLocation(new BeatSaberInstall(Path.GetFullPath(s), InstallType.Manual));
                    },
                    s =>
                    {
                        return NewLocationIsValid;
                    });

                return _addInstall;
            }
        }

        private string _newLocationInput = "";
        public string NewLocationInput
        {
            get { return _newLocationInput; }
            set
            {
                if (_newLocationInput == value)
                    return;
                NotifyLocationTimer.Stop();
                var oldVal = _newLocationInput;
                _newLocationInput = value;
                if(PathDidChange(oldVal, value))
                    NewLocationIsValid = false;
                NotifyLocationTimer.Start();
            }
        }

        private bool PathDidChange(string oldPath, string newPath)
        {
            if (string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath))
                return true;
            var directorySeparators = new string[] { Path.DirectorySeparatorChar.ToString(), Path.AltDirectorySeparatorChar.ToString() };
            var oldPathLastChar = oldPath.Substring(oldPath.Length - 1);
            var newPathLastChar = newPath.Substring(newPath.Length - 1);
            if (directorySeparators.Any(s => s.Equals(oldPathLastChar)))
                return true;
            if (oldPath.Equals(newPath.Substring(0, newPath.Length - 1)) && directorySeparators.Any(s => s.Equals(newPathLastChar)))
                return false;
            return true;
        }

        private bool _newLocationIsValid;
        public bool NewLocationIsValid
        {
            get { return _newLocationIsValid; }
            private set
            {
                var oldValue = _newLocationIsValid;
                if (_newLocationIsValid == value)
                    return;
                if (!value)
                    _newLocationIsValid = false;
                else
                    _newLocationIsValid = value;
                if (oldValue == _newLocationIsValid)
                    return;
                if (!_newLocationIsValid)
                {
                    AddInstall.RaiseCanExecuteChanged();
                    NotifyPropertyChanged();
                }
            }
        }

        private TimeSpan _notifyLocationTimerInterval = new TimeSpan(0, 0, 0, 0, 500);

        private DispatcherTimer _notifyLocationTimer;
        private DispatcherTimer NotifyLocationTimer
        {
            get
            {
                if (_notifyLocationTimer == null)
                {
                    _notifyLocationTimer = new DispatcherTimer();
                    _notifyLocationTimer.Interval = _notifyLocationTimerInterval;
                    _notifyLocationTimer.Tick += _notifyLocationTimer_Tick;
                }
                return _notifyLocationTimer;
            }
        }

        private void _notifyLocationTimer_Tick(object sender, EventArgs e)
        {
            NotifyLocationTimer.Stop();
            _newLocationIsValid = CanAddLocation(NewLocationInput);
            AddInstall.RaiseCanExecuteChanged();
            NotifyPropertyChanged("NewLocationIsValid");
        }

        private bool AddLocation(BeatSaberInstall beatSaberInstall)
        {
            BeatSaberLocations.Add(beatSaberInstall);
            return true;
        }

        public bool CanAddLocation(string pathStr)
        {
            if (string.IsNullOrEmpty(pathStr))
            {
                return false;
            }
            try
            {
                var fullInstallPath = Path.GetFullPath(pathStr).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (!Directory.Exists(fullInstallPath))
                    return false;
                if (BeatSaberLocations.Any(i => string.Equals(fullInstallPath, Path.GetFullPath(i.InstallPath), StringComparison.CurrentCultureIgnoreCase)))
                {
                    return false;
                }
            }
            catch { return false; }

            return true;
        }
    }
}
