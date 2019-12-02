using BeatSaberModdingTools.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BeatSaberModdingTools.Utilities.Paths;

namespace BeatSaberModdingTools.Utilities
{
    public static class BeatSaberTools
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
                        if (IsBeatSaberDirectory(path))
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
            DirectoryInfo bsDir = null;
            try
            {
                bsDir = new DirectoryInfo(path);
            }
            catch { return false; }
            if (bsDir.Exists)
            {
                var files = bsDir.GetFiles("Beat Saber.exe");
                return files.Count() > 0;
            }
            return false;
        }

        public static List<ReferenceModel> GetAvailableReferences(string beatSaberDir)
        {
            var retList = new List<ReferenceModel>();
            if (string.IsNullOrEmpty(beatSaberDir) || !Directory.Exists(beatSaberDir)) return retList;
            var libraryPaths = new string[] { Path_Managed, Path_Libs, Path_Plugins };
            foreach (var path in libraryPaths)
            {
                try
                {
                    var fullPath = new DirectoryInfo(Path.Combine(beatSaberDir, path));
                    if (fullPath.Exists)
                    {
                        foreach (var item in fullPath.GetFiles("*.dll"))
                        {
                            ReferenceModel refItem = null;
                            refItem = CreateReferenceFromFile(item.FullName);

                            retList.Add(refItem);
                            if (refItem != null)
                            {
                                refItem.RelativeDirectory = path;
                            }
                        }
                    }
                }
                catch { }
            }

            return retList;
        }

        public static ReferenceModel CreateReferenceFromFile(string fileName)
        {
            //var assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(fileName);
            //var assemblyName = assembly.GetName();
            string version = FileVersionInfo.GetVersionInfo(fileName).FileVersion;
            var refItem = new ReferenceModel(Path.GetFileNameWithoutExtension(fileName))
            {
                Version = version,
                HintPath = fileName
            };

            return refItem;
        }
    }
}
