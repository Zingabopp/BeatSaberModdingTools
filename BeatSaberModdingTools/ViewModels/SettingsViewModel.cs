using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            CurrentSettings = new SettingsModel(PreviousSettings);
            ExecuteOnAppSettingsChange = new WeakAction(() =>
            {
                NotifyPropertyChanged(string.Empty);
            });
            BSMTSettingsManager.SubscribeExecuteOnChange(ExecuteOnAppSettingsChange);
        }

        private ReadOnlySettingsModel PreviousSettings => BSMTSettingsManager.CurrentSettings;
        private SettingsModel _currentSettings;
        private WeakAction ExecuteOnAppSettingsChange;

        public SettingsModel CurrentSettings
        {
            get { return _currentSettings; }
            set
            {
                if (_currentSettings?.Equals(value) ?? value == null)
                    return;
                _currentSettings = value;
                NotifyPropertyChanged();
            }
        }

        #region Settings Properties
        public bool ChosenInstallPathChanged => CurrentSettings.ChosenInstallPath != PreviousSettings.ChosenInstallPath;
        public string ChosenInstallPath
        {
            get { return CurrentSettings?.ChosenInstallPath ?? PreviousSettings.ChosenInstallPath; }
            set
            {
                if (CurrentSettings?.ChosenInstallPath == value)
                    return;
                bool oldChangedVal = ChosenInstallPathChanged;
                CurrentSettings.ChosenInstallPath = value;
                NotifyPropertyChanged();
                if (oldChangedVal != ChosenInstallPathChanged)
                    NotifyPropertyChanged(nameof(ChosenInstallPathChanged));
            }
        }

        public bool GenerateUserFileWithTemplateChanged => CurrentSettings.GenerateUserFileWithTemplate != PreviousSettings.GenerateUserFileWithTemplate;
        public bool GenerateUserFileWithTemplate
        {
            get { return CurrentSettings?.GenerateUserFileWithTemplate ?? PreviousSettings.GenerateUserFileWithTemplate; }
            set
            {
                if (CurrentSettings?.GenerateUserFileWithTemplate == value)
                    return;
                bool oldChangedVal = GenerateUserFileWithTemplateChanged;
                CurrentSettings.GenerateUserFileWithTemplate = value;
                NotifyPropertyChanged();
                if (oldChangedVal != GenerateUserFileWithTemplateChanged)
                    NotifyPropertyChanged(nameof(GenerateUserFileWithTemplateChanged));
            }
        }

        public bool GenerateUserFileOnExistingChanged => CurrentSettings.GenerateUserFileOnExisting != PreviousSettings.GenerateUserFileOnExisting;
        public bool GenerateUserFileOnExisting
        {
            get { return CurrentSettings?.GenerateUserFileOnExisting ?? PreviousSettings.GenerateUserFileOnExisting; }
            set
            {
                if (CurrentSettings?.GenerateUserFileOnExisting == value)
                    return;
                bool oldChangedVal = GenerateUserFileOnExistingChanged;
                CurrentSettings.GenerateUserFileOnExisting = value;
                NotifyPropertyChanged();
                if (oldChangedVal != GenerateUserFileOnExistingChanged)
                    NotifyPropertyChanged(nameof(GenerateUserFileOnExistingChanged));
            }
        }

        public bool SetManifestJsonDefaultsChanged => CurrentSettings.SetManifestJsonDefaults != PreviousSettings.SetManifestJsonDefaults;
        public bool SetManifestJsonDefaults
        {
            get { return CurrentSettings?.SetManifestJsonDefaults ?? PreviousSettings.SetManifestJsonDefaults; }
            set
            {
                if (CurrentSettings?.SetManifestJsonDefaults == value)
                    return;
                bool oldChangedVal = SetManifestJsonDefaultsChanged;
                CurrentSettings.SetManifestJsonDefaults = value;
                NotifyPropertyChanged();
                if (oldChangedVal != SetManifestJsonDefaultsChanged)
                    NotifyPropertyChanged(nameof(SetManifestJsonDefaultsChanged));
            }
        }

        public bool CopyToIPAPendingOnBuildChanged => CurrentSettings.CopyToIPAPendingOnBuild != PreviousSettings.CopyToIPAPendingOnBuild;
        public bool CopyToIPAPendingOnBuild
        {
            get { return CurrentSettings?.CopyToIPAPendingOnBuild ?? PreviousSettings.CopyToIPAPendingOnBuild; }
            set
            {
                if (CurrentSettings?.CopyToIPAPendingOnBuild == value)
                    return;
                bool oldChangedVal = CopyToIPAPendingOnBuildChanged;
                CurrentSettings.CopyToIPAPendingOnBuild = value;
                NotifyPropertyChanged();
                if (oldChangedVal != CopyToIPAPendingOnBuildChanged)
                    NotifyPropertyChanged(nameof(CopyToIPAPendingOnBuildChanged));
            }
        }

        public bool BuildReferenceTypeChanged => CurrentSettings.BuildReferenceType != PreviousSettings.BuildReferenceType;
        public BuildReferenceType BuildReferenceType
        {
            get { return CurrentSettings?.BuildReferenceType ?? PreviousSettings.BuildReferenceType; }
            set
            {
                if (CurrentSettings?.BuildReferenceType == value)
                    return;
                bool oldChangedVal = BuildReferenceTypeChanged;
                CurrentSettings.BuildReferenceType = value;
                NotifyPropertyChanged();
                if (oldChangedVal != BuildReferenceTypeChanged)
                    NotifyPropertyChanged(nameof(BuildReferenceTypeChanged));
            }
        }

        public bool Manifest_AuthorChanged => CurrentSettings.Manifest_Author != PreviousSettings.Manifest_Author;
        public string Manifest_Author
        {
            get { return CurrentSettings?.Manifest_Author ?? PreviousSettings.Manifest_Author; }
            set
            {
                if (CurrentSettings?.Manifest_Author == value)
                    return;
                bool oldChangedVal = Manifest_AuthorChanged;
                CurrentSettings.Manifest_Author = value;
                NotifyPropertyChanged();
                if (oldChangedVal != Manifest_AuthorChanged)
                    NotifyPropertyChanged(nameof(Manifest_AuthorChanged));
            }
        }

        public bool Manifest_DonationChanged => CurrentSettings.Manifest_Donation != PreviousSettings.Manifest_Donation;
        public string Manifest_Donation
        {
            get { return CurrentSettings?.Manifest_Donation ?? PreviousSettings.Manifest_Donation; }
            set
            {
                if (CurrentSettings?.Manifest_Donation == value)
                    return;
                bool oldChangedVal = Manifest_DonationChanged;
                CurrentSettings.Manifest_Donation = value;
                NotifyPropertyChanged();
                if (oldChangedVal != Manifest_DonationChanged)
                    NotifyPropertyChanged(nameof(Manifest_DonationChanged));
            }
        }

        public bool Manifest_AuthorEnabledChanged => CurrentSettings.Manifest_AuthorEnabled != PreviousSettings.Manifest_AuthorEnabled;
        public bool Manifest_AuthorEnabled
        {
            get { return CurrentSettings?.Manifest_AuthorEnabled ?? PreviousSettings.Manifest_AuthorEnabled; }
            set
            {
                if (CurrentSettings?.Manifest_AuthorEnabled == value)
                    return;
                bool oldChangedVal = Manifest_AuthorEnabledChanged;
                CurrentSettings.Manifest_AuthorEnabled = value;
                NotifyPropertyChanged();
                if (oldChangedVal != Manifest_AuthorEnabledChanged)
                    NotifyPropertyChanged(nameof(Manifest_AuthorEnabledChanged));
            }
        }

        public bool Manifest_DonationEnabledChanged => CurrentSettings.Manifest_DonationEnabled != PreviousSettings.Manifest_DonationEnabled;
        public bool Manifest_DonationEnabled
        {
            get { return CurrentSettings?.Manifest_DonationEnabled ?? PreviousSettings.Manifest_DonationEnabled; }
            set
            {
                if (CurrentSettings?.Manifest_DonationEnabled == value)
                    return;
                bool oldChangedVal = Manifest_DonationEnabledChanged;
                CurrentSettings.Manifest_DonationEnabled = value;
                NotifyPropertyChanged();
                if (oldChangedVal != Manifest_DonationEnabledChanged)
                    NotifyPropertyChanged(nameof(Manifest_DonationEnabledChanged));
            }
        }
        #endregion

    }
}
