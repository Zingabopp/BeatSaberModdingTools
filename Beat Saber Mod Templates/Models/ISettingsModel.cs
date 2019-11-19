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
    }
}
