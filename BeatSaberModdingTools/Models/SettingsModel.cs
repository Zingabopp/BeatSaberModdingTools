using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Models
{
    public class SettingsModel : SettingsBase, IEquatable<ISettingsModel>
    {

        public override string ChosenInstallPath { get; set; }
        public override bool GenerateUserFileWithTemplate { get; set; }
        public override bool GenerateUserFileOnExisting { get; set; }
        public override bool SetManifestJsonDefaults { get; set; }
        public override bool CopyToIPAPendingOnBuild { get; set; }
        public override BuildReferenceType BuildReferenceType { get; set; }
        public override string Manifest_Author { get; set; }
        public override string Manifest_Donation { get; set; }

        public override bool Manifest_AuthorEnabled { get; set; }

        public override bool Manifest_DonationEnabled { get; set; }

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
            Manifest_AuthorEnabled = settingsModel.Manifest_AuthorEnabled;
            Manifest_DonationEnabled = settingsModel.Manifest_DonationEnabled;
        }

        public override bool Equals(object other)
        {
            if (other is ISettingsModel settings)
                return Equals(settings);
            else
                return false;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            int hashCode = 57113324;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ChosenInstallPath);
            hashCode = hashCode * -1521134295 + GenerateUserFileWithTemplate.GetHashCode();
            hashCode = hashCode * -1521134295 + GenerateUserFileOnExisting.GetHashCode();
            hashCode = hashCode * -1521134295 + SetManifestJsonDefaults.GetHashCode();
            hashCode = hashCode * -1521134295 + CopyToIPAPendingOnBuild.GetHashCode();
            hashCode = hashCode * -1521134295 + BuildReferenceType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Manifest_Author);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Manifest_Donation);
            hashCode = hashCode * -1521134295 + Manifest_AuthorEnabled.GetHashCode();
            hashCode = hashCode * -1521134295 + Manifest_DonationEnabled.GetHashCode();
            return hashCode;
        }
    }
}
