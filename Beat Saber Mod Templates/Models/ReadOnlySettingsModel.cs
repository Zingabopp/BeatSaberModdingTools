using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates.Models
{
    public class ReadOnlySettingsModel : ICloneable, IEquatable<SettingsModel>
    {

        public virtual string ChosenInstallPath { get; }

        public bool GenerateUserFileWithTemplate { get; protected set; }

        public bool GenerateUserFileOnExisting { get; protected set; }

        public bool SetManifestJsonDefaults { get; protected set; }

        public bool CopyToIPAPendingOnBuild { get; protected set; }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public SettingsModel Clone()
        {
            var cloned = new SettingsModel()
            {
                ChosenInstallPath = ChosenInstallPath,
                GenerateUserFileWithTemplate = GenerateUserFileWithTemplate,
                GenerateUserFileOnExisting = GenerateUserFileOnExisting,
                SetManifestJsonDefaults = SetManifestJsonDefaults,
                CopyToIPAPendingOnBuild = CopyToIPAPendingOnBuild
            };
            return cloned;
        }

        public bool Equals(SettingsModel other)
        {
            return GenerateUserFileWithTemplate == other.GenerateUserFileWithTemplate
                && GenerateUserFileOnExisting == other.GenerateUserFileOnExisting
                && SetManifestJsonDefaults == other.SetManifestJsonDefaults
                && CopyToIPAPendingOnBuild == other.CopyToIPAPendingOnBuild
                && ChosenInstallPath == other.ChosenInstallPath;
        }
    }
}
