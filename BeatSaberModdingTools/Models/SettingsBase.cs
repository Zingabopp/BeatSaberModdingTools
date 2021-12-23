using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Models
{
    public abstract class SettingsBase : ISettingsModel
    {
        public abstract string ChosenInstallPath { get; set; }
        public abstract bool GenerateUserFileWithTemplate { get; set; }
        public abstract bool GenerateUserFileOnExisting { get; set; }
        public abstract bool SetManifestJsonDefaults { get; set; }
        public abstract bool CopyToIPAPendingOnBuild { get; set; }
        public abstract BuildReferenceType BuildReferenceType { get; set; }
        public abstract string Manifest_Author { get; set; }
        public abstract string Manifest_Donation { get; set; }
        public abstract bool Manifest_AuthorEnabled { get; set; }
        public abstract bool Manifest_DonationEnabled { get; set; }

        public virtual void Populate(ISettingsModel other)
        {
            GenerateUserFileWithTemplate = other.GenerateUserFileWithTemplate;
            GenerateUserFileOnExisting = other.GenerateUserFileOnExisting;
            SetManifestJsonDefaults = other.SetManifestJsonDefaults;
            CopyToIPAPendingOnBuild = other.CopyToIPAPendingOnBuild;
            BuildReferenceType = other.BuildReferenceType;
            ChosenInstallPath = other.ChosenInstallPath;
            Manifest_Author = other.Manifest_Author;
            Manifest_Donation = other.Manifest_Donation;
            Manifest_AuthorEnabled = other.Manifest_AuthorEnabled;
            Manifest_DonationEnabled = other.Manifest_DonationEnabled;
        }

        public virtual bool Equals(ISettingsModel other)
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

        public override bool Equals(object other)
        {
            if (other is ISettingsModel settings)
                return Equals(settings);
            else
                return false;
        }
    }
}
