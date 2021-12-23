using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Models
{
    public interface ISettingsModel : IEquatable<ISettingsModel>
    {
        string ChosenInstallPath { get; set; }

        bool GenerateUserFileWithTemplate { get; set; }

        bool GenerateUserFileOnExisting { get; set; }

        bool SetManifestJsonDefaults { get; set; }

        bool CopyToIPAPendingOnBuild { get; set; }

        BuildReferenceType BuildReferenceType { get; set; }

        string Manifest_Author { get; set; }

        string Manifest_Donation { get; set; }

        bool Manifest_AuthorEnabled { get; set; }

        bool Manifest_DonationEnabled { get; set; }
        void Populate(ISettingsModel other);
    }

    public enum BuildReferenceType : byte
    {
        UserFile = 0,
        BuildTools = 1,
        DirectoryJunctions = 2
    }
}
