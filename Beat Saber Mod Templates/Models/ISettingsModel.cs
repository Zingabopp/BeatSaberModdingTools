using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates.Models
{
    public interface ISettingsModel : ICloneable, IEquatable<ISettingsModel>
    {
        string ChosenInstallPath { get; }

        bool GenerateUserFileWithTemplate { get; }

        bool GenerateUserFileOnExisting { get; }

        bool SetManifestJsonDefaults { get; }

        bool CopyToIPAPendingOnBuild { get; }

        BuildReferenceType BuildReferenceType { get; }

        string Manifest_Author { get; }

        string Manifest_Donation { get; }

        bool Manifest_AuthorEnabled { get; }

        bool Manifest_DonationEnabled { get; }
    }

    public enum BuildReferenceType : byte
    {
        UserFile = 0,
        BuildTools = 1,
        DirectoryJunctions = 2
    }
}
