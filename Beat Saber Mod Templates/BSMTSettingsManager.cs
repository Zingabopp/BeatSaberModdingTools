using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberModTemplates.Models;
using Microsoft.VisualStudio.Shell;

namespace BeatSaberModTemplates
{
    public static class BSMTSettingsManager
    {
        static Properties.BeatSaberModTemplatesSettings Settings => Properties.BeatSaberModTemplatesSettings.Default;
        public static ReadOnlySettingsModel CurrentSettings { get; private set; }

        static BSMTSettingsManager()
        {
            UpdateCurrentSettings();
        }

        public static void Initialize()
        {
            if (CurrentSettings == null)
                UpdateCurrentSettings();
        }

        public static void Store(ISettingsModel newSettings)
        {
            CurrentSettings = new ReadOnlySettingsModel(newSettings);
            Settings.ChosenInstallPath = CurrentSettings.ChosenInstallPath;
            Settings.GenerateUserFileWithTemplate = CurrentSettings.GenerateUserFileWithTemplate;
            Settings.GenerateUserFileOnExisting = CurrentSettings.GenerateUserFileOnExisting;
            Settings.SetManifestJsonDefaults = CurrentSettings.SetManifestJsonDefaults;
            Settings.CopyToIPAPendingOnBuild = CurrentSettings.CopyToIPAPendingOnBuild;
            Settings.BuildReferenceType = (byte)newSettings.BuildReferenceType;
            Settings.Manifest_Author = newSettings.Manifest_Author;
            Settings.Manifest_Donation = newSettings.Manifest_Donation;
            Settings.Save();
        }

        public static void Reload()
        {
            Settings.Reload();
            UpdateCurrentSettings();
        }

        public static void UpdateCurrentSettings()
        {
            try
            {
                CurrentSettings = new ReadOnlySettingsModel(Settings.ChosenInstallPath, Settings.GenerateUserFileWithTemplate, Settings.GenerateUserFileOnExisting,
                     Settings.SetManifestJsonDefaults, Settings.CopyToIPAPendingOnBuild, (BuildReferenceType)Settings.BuildReferenceType, Settings.Manifest_Author, Settings.Manifest_Donation);
            }
            catch (NullReferenceException)
            {
                CurrentSettings = new ReadOnlySettingsModel();
            }
        }
    }
}
