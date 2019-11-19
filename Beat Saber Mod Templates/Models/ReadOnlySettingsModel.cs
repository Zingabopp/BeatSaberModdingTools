using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates.Models
{
    public class ReadOnlySettingsModel : ISettingsModel
    {

        public string ChosenInstallPath { get; }

        public bool GenerateUserFileWithTemplate { get; }

        public bool GenerateUserFileOnExisting { get; }

        public bool SetManifestJsonDefaults { get; }

        public bool CopyToIPAPendingOnBuild { get; }

        public ReadOnlySettingsModel()
        {
            ChosenInstallPath = string.Empty;
        }

        public ReadOnlySettingsModel(string chosenPath, bool genUserWithTemp, bool genUserExisting, bool setManDefaults, bool copyToPending)
        {
            ChosenInstallPath = chosenPath;
            GenerateUserFileWithTemplate = genUserWithTemp;
            GenerateUserFileOnExisting = genUserExisting;
            SetManifestJsonDefaults = setManDefaults;
            CopyToIPAPendingOnBuild = copyToPending;
        }

        public ReadOnlySettingsModel(ISettingsModel settingsModel)
        {
            ChosenInstallPath = settingsModel.ChosenInstallPath;
            GenerateUserFileWithTemplate = settingsModel.GenerateUserFileWithTemplate;
            GenerateUserFileOnExisting = settingsModel.GenerateUserFileOnExisting;
            SetManifestJsonDefaults = settingsModel.SetManifestJsonDefaults;
            CopyToIPAPendingOnBuild = settingsModel.CopyToIPAPendingOnBuild;
        }

        public bool Equals(ISettingsModel other)
        {
            return GenerateUserFileWithTemplate == other.GenerateUserFileWithTemplate
                && GenerateUserFileOnExisting == other.GenerateUserFileOnExisting
                && SetManifestJsonDefaults == other.SetManifestJsonDefaults
                && CopyToIPAPendingOnBuild == other.CopyToIPAPendingOnBuild
                && ChosenInstallPath == other.ChosenInstallPath;
        }

        object ICloneable.Clone()
        {
            return new ReadOnlySettingsModel(this);
        }
    }
}
