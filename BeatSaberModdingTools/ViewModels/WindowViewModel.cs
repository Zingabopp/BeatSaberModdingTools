using BeatSaberModdingTools.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BeatSaberModdingTools.Utilities;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace BeatSaberModdingTools.ViewModels
{
    public class WindowViewModel : ViewModelBase
    {
        INotificationHandler _notificationHandler;
        public ObservableCollection<BeatSaberInstall> DesignExample => new ObservableCollection<BeatSaberInstall>()
        {
            new BeatSaberInstall(@"C:\SteamInstall", InstallType.Steam),
            new BeatSaberInstall(@"C:\OculusInstall\DDDDDDDDDD\AAAAAAAAAA\VVVVVVVVVVVV\CCCCCCCCCCCCCC\SSSSSSSSSSSSSS\F", InstallType.Oculus),
            new BeatSaberInstall(@"C:\ManualInstall", InstallType.Manual)
        };

        public WindowViewModel(INotificationHandler notificationHandler)
        {
            if (BSMTSettingsManager.Instance == null)
                BSMTSettingsManager.UseDefaultManager();
            _notificationHandler = notificationHandler;
            try
            {
                SettingsViewModel = new SettingsViewModel();
                //AddLocation(new BeatSaberInstall(@"C:\SteamInstall", InstallType.Steam));
                //AddLocation(new BeatSaberInstall(@"C:\OculusInstall\DDDDDDDDDD\AAAAAAAAAA\VVVVVVVVVVVV\CCCCCCCCCCCCCC\SSSSSSSSSSSSSS\F", InstallType.Oculus));
                //AddLocation(new BeatSaberInstall(@"C:\ManualInstall", InstallType.Manual));
                var detectedLocations = BeatSaberTools.GetBeatSaberPathsFromRegistry();
                BeatSaberLocations = new ObservableCollection<BeatSaberInstall>(detectedLocations);
                SetInstallByPath(SettingsViewModel.CurrentSettings.ChosenInstallPath);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                if (BeatSaberLocations == null)
                {
                    BeatSaberLocations = new ObservableCollection<BeatSaberInstall>();
                    SetInstallByPath(SettingsViewModel.CurrentSettings.ChosenInstallPath);
                }
            }
        }

        public void SetInstallByPath(string path)
        {
            try
            {
                path = Path.GetFullPath(path);
            }
            catch { }
            if (!string.IsNullOrEmpty(path))
            {
                var matchingInstall = BeatSaberLocations.Where(i => string.Equals(Path.GetFullPath(i.InstallPath), path, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (matchingInstall != null)
                    ChosenInstall = matchingInstall;
                else
                {
                    var newInstall = new BeatSaberInstall(path, InstallType.Manual);
                    BeatSaberLocations.Add(newInstall);
                    ChosenInstall = newInstall;
                }
            }
            else
            {
                ChosenInstall = BeatSaberLocations.FirstOrDefault();
            }
        }

        #region Public Properties
        public SettingsViewModel SettingsViewModel { get; set; }

        private ObservableCollection<BeatSaberInstall> _beatSaberLocations;
        /// <summary>
        /// An <see cref="ObservableCollection{T}"/> of <see cref="BeatSaberInstall"/>.
        /// </summary>
        public ObservableCollection<BeatSaberInstall> BeatSaberLocations
        {
            get { return _beatSaberLocations; }
            set
            {
                if (_beatSaberLocations == value)
                    return;
                if (_beatSaberLocations != null)
                    _beatSaberLocations.CollectionChanged -= BeatSaberLocations_CollectionChanged;
                _beatSaberLocations = value;
                _beatSaberLocations.CollectionChanged += BeatSaberLocations_CollectionChanged;
            }
        }

        private void BeatSaberLocations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (e.OldItems.Contains(ChosenInstall))
                    ChosenInstall = BeatSaberLocations.FirstOrDefault();
                AddInstall.RaiseCanExecuteChanged();
            }
        }


        private BeatSaberInstall _chosenInstall;
        /// <summary>
        /// The currently used install location.
        /// </summary>
        public BeatSaberInstall ChosenInstall
        {
            get { return _chosenInstall ?? BeatSaberLocations.FirstOrDefault(); }
            set
            {
                if (_chosenInstall == value)
                    return;
                _chosenInstall = value;
                SettingsViewModel.ChosenInstallPath = _chosenInstall?.InstallPath ?? string.Empty;
                NotifyPropertyChanged();
            }
        }

        private string _newLocationInput = "";
        /// <summary>
        /// Contains the user-provided path intended to be added to <see cref="BeatSaberLocations"/>.
        /// When changed, restarts <see cref="NotifyLocationTimer"/> and sets <see cref="NewLocationIsValid"/> to false immediately.
        /// </summary>
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
                if (PathDidChange(oldVal, value))
                    NewLocationIsValid = false;
                NotifyLocationTimer.Start();
                NotifyPropertyChanged();
            }
        }
        private bool _newLocationIsValid;
        public bool NewLocationIsValid
        {
            get { return _newLocationIsValid; }
            private set
            {
                if (_newLocationIsValid == value)
                    return;
                _newLocationIsValid = value;
                if (!_newLocationIsValid)
                {
                    AddInstall.RaiseCanExecuteChanged();
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion
        #region Commands

        private RelayCommand<string> _addInstall;
        /// <summary>
        /// Adds the specified location to <see cref="BeatSaberLocations"/> as <see cref="InstallType.Manual"/>.
        /// </summary>
        public RelayCommand<string> AddInstall
        {
            get
            {
                if (_addInstall == null)
                    _addInstall = new RelayCommand<string>(s =>
                    {
                        AddLocation(new BeatSaberInstall(Path.GetFullPath(s), InstallType.Manual));
                        _addInstall.RaiseCanExecuteChanged();
                    },
                    s =>
                    {
                        NewLocationIsValid = CanAddLocation(s);
                        return NewLocationIsValid;
                    });

                return _addInstall;
            }
        }

        private RelayCommand<BeatSaberInstall> _removeInstall;
        /// <summary>
        /// Adds the specified location to <see cref="BeatSaberLocations"/> as <see cref="InstallType.Manual"/>.
        /// </summary>
        public RelayCommand<BeatSaberInstall> RemoveInstall
        {
            get
            {
                if (_removeInstall == null)
                    _removeInstall = new RelayCommand<BeatSaberInstall>(i =>
                    {
                        if (i == null)
                            return;
                        RemoveLocation(i);
                    },
                    i =>
                    {
                        if (i == null)
                            return false;
                        return i.InstallType == InstallType.Manual && BeatSaberLocations.Contains(i);
                    });

                return _removeInstall;
            }
        }
        #endregion
        private void ShowError(string title, string message)
        {
            _notificationHandler.ShowError(title, message);
        }
        private void ShowError(Exception ex, [CallerMemberName] string title = null)
        {
            _notificationHandler?.ShowError(title, $"{ex.Message}\n{ex.StackTrace}");
        }
        private bool PathDidChange(string oldPath, string newPath)
        {
            try
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
            catch (Exception ex)
            {
                ShowError("PathDidChange", $"{ex.Message}\n{ex.StackTrace}");
                return true;
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
            bool hadEmptyList = BeatSaberLocations.Count == 0;
            BeatSaberLocations.Add(beatSaberInstall);
            if (BeatSaberLocations.Any(i => i.InstallPath == NewLocationInput.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)))
                NewLocationIsValid = false;
            if (hadEmptyList)
                ChosenInstall = beatSaberInstall;
            return true;
        }

        private bool RemoveLocation(BeatSaberInstall install)
        {
            bool result = BeatSaberLocations.Remove(install);
            if (ChosenInstall == install)
                ChosenInstall = null;
            NewLocationIsValid = CanAddLocation(NewLocationInput);
            return result;
        }

        public bool CanAddLocation(string pathStr)
        {
            if (string.IsNullOrEmpty(pathStr))
            {
                return false;
            }
            try
            {
                pathStr = pathStr.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var fullInstallPath = Path.GetFullPath(pathStr).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (Directory.Exists(fullInstallPath) && Path.IsPathRooted(pathStr) && !pathStr.EndsWith(":"))
                {
                    bool locationExists = BeatSaberLocations.Any(i =>
                       {
                           var comp = Path.GetFullPath(i.InstallPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                           return string.Equals(fullInstallPath, comp, StringComparison.CurrentCultureIgnoreCase);
                       });
                    if (!locationExists)
                    {
                        return true;
                    }
                }
            }
            catch { return false; }
            return false;
        }
    }
}
