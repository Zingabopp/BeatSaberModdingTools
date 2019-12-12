using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Models
{
    public class ActiveSettings : ISettingsModel
    {
        private Properties.BeatSaberModdingToolsSettings Settings => Properties.BeatSaberModdingToolsSettings.Default;
        public string ChosenInstallPath
        {
            get { return Settings.ChosenInstallPath; }
            set { Settings.ChosenInstallPath = value; }
        }

        public bool GenerateUserFileWithTemplate
        {
            get { return Settings.GenerateUserFileWithTemplate; }
            set { Settings.GenerateUserFileWithTemplate = value; }
        }

        public bool GenerateUserFileOnExisting
        {
            get { return Settings.GenerateUserFileOnExisting; }
            set { Settings.GenerateUserFileOnExisting = value; }
        }

        public bool SetManifestJsonDefaults
        {
            get { return Settings.SetManifestJsonDefaults; }
            set { Settings.SetManifestJsonDefaults = value; }
        }

        public bool CopyToIPAPendingOnBuild
        {
            get { return Settings.CopyToIPAPendingOnBuild; }
            set { Settings.CopyToIPAPendingOnBuild = value; }
        }

        public BuildReferenceType BuildReferenceType
        {
            get { return (BuildReferenceType)Settings.BuildReferenceType; }
            set { Settings.BuildReferenceType = (byte)value; }
        }

        public string Manifest_Author
        {
            get { return Settings.Manifest_Author; }
            set { Settings.Manifest_Author = value; }
        }

        public string Manifest_Donation
        {
            get { return Settings.Manifest_Donation; }
            set { Settings.Manifest_Donation = value; }
        }

        public bool Manifest_AuthorEnabled
        {
            get { return Settings.Manifest_AuthorEnabled; }
            set { Settings.Manifest_AuthorEnabled = value; }
        }

        public bool Manifest_DonationEnabled
        {
            get { return Settings.Manifest_DonationEnabled; }
            set { Settings.Manifest_DonationEnabled = value; }
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
                && Manifest_Donation == other.Manifest_Donation
                && Manifest_AuthorEnabled == other.Manifest_AuthorEnabled
                && Manifest_DonationEnabled == other.Manifest_DonationEnabled;
        }

        object ICloneable.Clone()
        {
            return new ActiveSettings();
        }

        public override bool Equals(object other)
        {
            if (other is ISettingsModel settings)
                return Equals(settings);
            else
                return false;
        }
    }
}
