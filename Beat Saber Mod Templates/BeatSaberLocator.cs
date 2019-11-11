using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates
{
    public static class BeatSaberLocator
    {
        // Using Path.Combine makes it safe for regions that don't use '\' as a directory separator?
        private static readonly string STEAM_REG_KEY = Path.Combine("SOFTWARE", "Microsoft", "Windows", "CurrentVersion", "Uninstall", "Steam App 620980");
        //private const string STEAM_REG_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 620980";
        private static readonly string OCULUS_REG_KEY = Path.Combine("SOFTWARE", "WOW6432Node", "Oculus VR, LLC", "Oculus", "Config");
        //private const string OCULUS_REG_KEY = @"SOFTWARE\WOW6432Node\Oculus VR, LLC\Oculus\Config";
        public static BeatSaberInstall[] GetBeatSaberPathsFromRegistry()
        {
            var installList = new List<BeatSaberInstall>();
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))// Doesn't work in 32 bit mode without this
            {
                using (var steamKey = hklm?.OpenSubKey(STEAM_REG_KEY))
                {
                    var path = (string)steamKey?.GetValue("InstallLocation", string.Empty);
                    if (IsBeatSaberDirectory(path))
                        installList.Add(new BeatSaberInstall(path, InstallType.Steam));
                }
                using (var oculusKey = hklm?.OpenSubKey(OCULUS_REG_KEY))
                {
                    var path = (string)oculusKey?.GetValue("InitialAppLibrary", string.Empty);
                    if (!string.IsNullOrEmpty(path))
                    {
                        path = Path.Combine(path, "Software", "hyperbolic-magnetism-beat-saber");
                        if(IsBeatSaberDirectory(path))
                            installList.Add(new BeatSaberInstall(path, InstallType.Oculus));
                    }
                }
            }
            return installList.ToArray();
        }

        public static bool IsBeatSaberDirectory(string path)
        {
            if (string.IsNullOrEmpty(path?.Trim()))
                return false;
            DirectoryInfo bsDir = new DirectoryInfo(path);
            bool valid = bsDir.Exists;
            if (bsDir.Exists)
            {
                var files = bsDir.GetFiles("Beat Saber.exe");
                return files.Count() > 0;
            }
            return false;
        }
    }

    public struct BeatSaberInstall
    {
        public string Path;
        public InstallType InstallType;
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
        Steam,
        Oculus
    }
}
