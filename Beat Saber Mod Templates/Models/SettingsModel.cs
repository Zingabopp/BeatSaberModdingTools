using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates.Models
{
    public class SettingsModel
    {
        public BeatSaberInstall ChosenInstall { get; set; }

        public bool GenerateUserFileWithTemplate { get; set; }

        public bool GenerateUserFileOnExisting { get; set; }

        public bool SetManifestJsonDefaults { get; set; }


    }
}
