using BeatSaberModdingTools.Models;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static BeatSaberModdingTools.Utilities.Paths;

namespace BeatSaberModdingTools.Utilities
{
    public static class BeatSaberTools
    {
        /// <summary>
        /// Path to the Steam Beat Saber uninstall registry entry, not always present.
        /// </summary>
        private static readonly string STEAM_BS_UNINSTALL_REG_KEY = Path.Combine("SOFTWARE", "Microsoft", "Windows", "CurrentVersion", "Uninstall", "Steam App 620980");
        /// <summary>
        /// Path to config.vdf from the Steam install folder.
        /// </summary>
        private static readonly string STEAM_CONFIG_PATH = Path.Combine("config", "libraryfolders.vdf");

        private static readonly string STEAM_PATH_KEY = Path.Combine("SOFTWARE", "WOW6432Node", "Valve", "Steam");

        //private const string STEAM_REG_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 620980";
        private static readonly string OCULUS_LM_KEY = Path.Combine("SOFTWARE", "WOW6432Node", "Oculus VR, LLC", "Oculus", "Config");
        private static readonly string OCULUS_CU_KEY = Path.Combine("SOFTWARE", "Oculus VR, LLC", "Oculus", "Libraries");
        //private const string OCULUS_REG_KEY = @"SOFTWARE\WOW6432Node\Oculus VR, LLC\Oculus\Config";
        public static BeatSaberInstall[] GetBeatSaberPathsFromRegistry()
        {
            List<BeatSaberInstall> installList = new List<BeatSaberInstall>();
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) // Doesn't work in 32 bit mode without this
            {
                string installedBS = GetInstalledSteamBeatSaber(hklm);
                if (installedBS != null)
                    installList.Add(new BeatSaberInstall(installedBS, InstallType.Steam));
                string[] steamLibraries = GetSteamLibraryPaths();
                foreach (string library in steamLibraries)
                {
                    string matchedLocation = FindBeatSaberInSteamLibrary(library);
                    if (!string.IsNullOrEmpty(matchedLocation)
                        && !matchedLocation.Equals(installedBS, StringComparison.OrdinalIgnoreCase))
                    {
                        installList.Add(new BeatSaberInstall(matchedLocation, InstallType.Steam));
                    }
                }
                string[] oculusLibraries = GetOculusLibraryPaths();
                foreach (string library in oculusLibraries)
                {
                    string matchedLocation = FindBeatSaberInOculusLibrary(library);
                    if (!string.IsNullOrEmpty(matchedLocation))
                        installList.Add(new BeatSaberInstall(matchedLocation, InstallType.Oculus));
                }
                string[] bsManagerInstances = GetBSManagerInstances();
                foreach (string instance in bsManagerInstances)
                {
                    if (File.Exists(Path.Combine(instance, "Beat Saber.exe")))
                    {
                        installList.Add(new BeatSaberInstall(instance, InstallType.BSManager));
                    }
                }
            }
            return installList.ToArray();
        }

        public static string GetInstalledSteamBeatSaber(RegistryKey hklm)
        {
            using (RegistryKey steamKey = hklm?.OpenSubKey(STEAM_BS_UNINSTALL_REG_KEY))
            {
                string path = (string)steamKey?.GetValue("InstallLocation", string.Empty);
                if (IsBeatSaberDirectory(path))
                    return path;
            }
            return null;
        }

        public static string FindBeatSaberInOculusLibrary(string oculusLibraryPath)
        {
            string possibleLocation = Path.Combine(oculusLibraryPath, "hyperbolic-magnetism-beat-saber");
            string matchedLocation = null;
            if (Directory.Exists(possibleLocation))
            {
                if (IsBeatSaberDirectory(possibleLocation))
                    return possibleLocation;
            }
            else
            {
                string softwareFolder = Path.Combine(oculusLibraryPath, "Software");
                if (Directory.Exists(softwareFolder))
                    matchedLocation = FindBeatSaberInOculusLibrary(softwareFolder);
            }
            return matchedLocation;
        }

        public static string FindBeatSaberInSteamLibrary(string steamLibraryPath)
        {
            string possibleLocation = Path.Combine(steamLibraryPath, "steamapps", "common", "Beat Saber");
            if (Directory.Exists(possibleLocation))
            {
                if (IsBeatSaberDirectory(possibleLocation))
                    return possibleLocation;
            }
            return null;
        }

        public static string[] GetBSManagerInstances()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFilePath = Path.Combine(appDataFolder, "bs-manager", "config.json");

            if (File.Exists(configFilePath))
            {
                try
                {
                    string configJson = File.ReadAllText(configFilePath);
                    dynamic configData = JsonConvert.DeserializeObject(configJson);

                    string installationFolder = configData["installation-folder"];
                    if (!string.IsNullOrEmpty(installationFolder))
                    {
                        string bsInstancesFolder = Path.Combine(installationFolder, "BSManager", "BSInstances");
                        if (Directory.Exists(bsInstancesFolder))
                        {
                            return Directory.GetDirectories(bsInstancesFolder).Select(folder => Path.GetFullPath(folder)).ToArray();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error reading config file: {e.Message}");
                }
            }

            return null;
        }

        public static string[] GetSteamLibraryPaths()
        {
            string[] libraryPaths = Array.Empty<string>();
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))// Doesn't work in 32 bit mode without this
            {
                using (RegistryKey steamKey = hklm?.OpenSubKey(STEAM_PATH_KEY))
                {
                    string path = (string)steamKey?.GetValue("InstallPath", string.Empty);
                    if (path != null && path.Length > 0)
                    {
                        string configPath = Path.Combine(path, STEAM_CONFIG_PATH);
                        if (File.Exists(configPath))
                        {
                            try
                            {
                                libraryPaths = LibrariesFromVdf(configPath);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Error reading Steam config file ('{configPath}'): {ex.Message}. Unable to find Steam Beat Saber installs.", ex);
                            }
                        }
                    }
                }
            }
            return libraryPaths;
        }

        public static string[] LibrariesFromVdf(string configPath)
        {
            string[] libraryPaths = null;
            string vdf = File.ReadAllText(configPath);

            VProperty v = VdfConvert.Deserialize(File.OpenText(configPath));
            VObject folderObject = v?.Value as VObject;
            VProperty[] folderInfo = folderObject?.Children<VProperty>()?.ToArray();
            if (folderInfo != null)
            {
                libraryPaths = folderInfo.Select((x) => (x.Value as VObject).Children<VProperty>().ToArray().Where((y) => y.Key == "path").Select((z) => z.Value.ToString()).First()).ToArray();
            }
            return libraryPaths ?? Array.Empty<string>();
        }

        public static string[] GetOculusLibraryPaths()
        {
            List<string> paths = new List<string>();
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) // Doesn't work in 32 bit mode without this
            {
                using (RegistryKey oculusKey = hklm?.OpenSubKey(OCULUS_LM_KEY))
                {
                    string path = (string)oculusKey?.GetValue("InitialAppLibrary", string.Empty);
                    if (!string.IsNullOrEmpty(path))
                    {
                        paths.Add(path);
                    }
                }
            }
            using (RegistryKey hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)) // Doesn't work in 32 bit mode without this
            {
                using (RegistryKey oculusKey = hkcu?.OpenSubKey(OCULUS_CU_KEY))
                {
                    if (oculusKey != null && oculusKey.SubKeyCount > 0)
                    {
                        foreach (string libraryKeyName in oculusKey.GetSubKeyNames())
                        {
                            using (RegistryKey library = oculusKey.OpenSubKey(libraryKeyName))
                            {
                                string path = (string)library?.GetValue("OriginalPath", string.Empty);
                                if (!string.IsNullOrEmpty(path) && !paths.Contains(path))
                                    paths.Add(path);
                            }
                        }
                    }
                }
            }
            return paths.ToArray();
        }

        public static readonly char[] IllegalCharacters = new char[]
            {
                '<', '>', ':', '/', '\\', '|', '?', '*', '"',
                '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007',
                '\u0008', '\u0009', '\u000a', '\u000b', '\u000c', '\u000d', '\u000e', '\u000d',
                '\u000f', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016',
                '\u0017', '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d', '\u001f',
            };
        /// <summary>
        /// Attempts to get the Beat Saber game version from the given install directory. Returns null if it fails.
        /// </summary>
        /// <remarks>
        /// Uses the implementation from https://github.com/Assistant/ModAssistant
        /// </remarks>
        /// <param name="gameDir"></param>
        /// <returns></returns>
        public static string GetVersion(string gameDir)
        {
            string filename = Path.Combine(gameDir, "Beat Saber_Data", "globalgamemanagers");
            if (!File.Exists(filename))
                return null;
            try
            {
                byte[] file = File.ReadAllBytes(filename);
                string str = Encoding.Default.GetString(file);
                string versionLocation = "public.app-category.games";
                int startIndex = str.IndexOf(versionLocation) + 136;
                int length = str.IndexOfAny(IllegalCharacters, startIndex) - startIndex;
                string version = str.Substring(startIndex, length);

                return version;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsBeatSaberDirectory(string path)
        {
            if (string.IsNullOrEmpty(path?.Trim()))
                return false;
            DirectoryInfo bsDir;
            try
            {
                bsDir = new DirectoryInfo(path);
            }
            catch { return false; }
            if (bsDir.Exists)
            {
                FileInfo[] files = bsDir.GetFiles("Beat Saber.exe");
                return files.Count() > 0;
            }
            return false;
        }

        public static List<ReferenceModel> GetAvailableReferences(string beatSaberDir)
        {
            List<ReferenceModel> retList = new List<ReferenceModel>();
            if (string.IsNullOrEmpty(beatSaberDir) || !Directory.Exists(beatSaberDir)) return retList;
            string[] libraryPaths = new string[] { Path_Managed, Path_Libs, Path_Plugins };
            foreach (string path in libraryPaths)
            {
                try
                {
                    DirectoryInfo fullPath = new DirectoryInfo(Path.Combine(beatSaberDir, path));
                    if (fullPath.Exists)
                    {
                        foreach (FileInfo item in fullPath.GetFiles("*.dll").ToArray())
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
            ReferenceModel refItem = new ReferenceModel(Path.GetFileNameWithoutExtension(fileName))
            {
                Version = version,
                HintPath = fileName
            };

            return refItem;
        }
    }
}
