using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates.Models
{
    public class SettingsModel : ISettingsModel
    {

        public string ChosenInstallPath { get; set; }

        public bool GenerateUserFileWithTemplate { get; set; }

        public bool GenerateUserFileOnExisting { get; set; }

        public bool SetManifestJsonDefaults { get; set; }

        public bool CopyToIPAPendingOnBuild { get; set; }

        public BuildReferenceType BuildReferenceType { get; set; }

        public string Manifest_Author { get; set; }

        public string Manifest_Donation { get; set; }

        public SettingsModel() 
        {
            ChosenInstallPath = string.Empty;
        }

        public SettingsModel(ISettingsModel settingsModel)
        {
            ChosenInstallPath = settingsModel.ChosenInstallPath;
            GenerateUserFileWithTemplate = settingsModel.GenerateUserFileWithTemplate;
            GenerateUserFileOnExisting = settingsModel.GenerateUserFileOnExisting;
            SetManifestJsonDefaults = settingsModel.SetManifestJsonDefaults;
            CopyToIPAPendingOnBuild = settingsModel.CopyToIPAPendingOnBuild;
            BuildReferenceType = settingsModel.BuildReferenceType;
            Manifest_Author = settingsModel.Manifest_Author;
            Manifest_Donation = settingsModel.Manifest_Donation;
        }

        object ICloneable.Clone()
        {
            return new SettingsModel(this);
        }

        public bool Equals(ISettingsModel other)
        {
            return GenerateUserFileWithTemplate == other.GenerateUserFileWithTemplate
                && GenerateUserFileOnExisting == other.GenerateUserFileOnExisting
                && SetManifestJsonDefaults == other.SetManifestJsonDefaults
                && CopyToIPAPendingOnBuild == other.CopyToIPAPendingOnBuild
                && BuildReferenceType == other.BuildReferenceType
                && ChosenInstallPath == other.ChosenInstallPath
                && Manifest_Author == other.Manifest_Author
                && Manifest_Donation == other.Manifest_Donation;
        }
    }
}
