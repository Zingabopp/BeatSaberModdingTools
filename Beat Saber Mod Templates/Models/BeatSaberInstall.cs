using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates.Models
{
    public class BeatSaberInstall
    {
        public string Path { get; private set; }
        public InstallType InstallType { get; private set; }

        public BeatSaberInstall(string path, InstallType installType)
        {
            Path = path;
            InstallType = installType;
        }
        public override string ToString()
        {
            return $"{InstallType.ToString()}: {Path}";
        }
    }

    public enum InstallType
    {
        Manual,
        Steam,
        Oculus
    }
}
