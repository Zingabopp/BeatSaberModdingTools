using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace BSMT_Tests.BuildTasks
{

    [TestClass]
    public class GetManifestInfo_Test
    {
        public static readonly string TestFilePath = Path.GetFullPath(@"Data\BuildTaskTests");
        [TestMethod]
        public void NormalManifest()
        {
            string manifestPath = Path.Combine(TestFilePath, "BeatSync_manifest.json");
            string assemblyPath = Path.Combine(TestFilePath, "BeatSync_AssemblyInfo.cs");
            string expectedVersion = "0.4.2";
            string expectedGameVer = "1.5.0";
            var task = new GetManifestInfoTask(manifestPath, assemblyPath);
            Assert.IsTrue(task.GetManifestInfo());
            Assert.AreEqual(expectedVersion, task.PluginVersion);
            Assert.AreEqual(expectedGameVer, task.GameVersion);
        }

        [TestMethod]
        public void WhiteSpace()
        {
            string manifestPath = Path.Combine(TestFilePath, "BeatSync_manifest_WhiteSpace.json");
            string assemblyPath = Path.Combine(TestFilePath, "BeatSync_AssemblyInfo.cs");
            string expectedVersion = "0.4.2";
            string expectedGameVer = "1.5.0";
            var task = new GetManifestInfoTask(manifestPath, assemblyPath);
            Assert.IsTrue(task.GetManifestInfo());
            Assert.AreEqual(expectedVersion, task.PluginVersion);
            Assert.AreEqual(expectedGameVer, task.GameVersion);
        }

        [TestMethod]
        public void MismatchedVersion()
        {
            string manifestPath = Path.Combine(TestFilePath, "BeatSync_manifest_WrongVersion.json");
            string assemblyPath = Path.Combine(TestFilePath, "BeatSync_AssemblyInfo.cs");
            var task = new GetManifestInfoTask(manifestPath, assemblyPath);
            Assert.IsTrue(task.GetManifestInfo());
            Assert.IsTrue(task.Log.Messages.First().Contains("BSMOD01"));
            Assert.IsTrue(task.Log.Messages.First().Contains("(35-35:29-34)"));
        }

        [TestMethod]
        public void MissingGameVer()
        {
            string manifestPath = Path.Combine(TestFilePath, "BeatSync_manifest_MissingGameVer.json");
            string assemblyPath = Path.Combine(TestFilePath, "BeatSync_AssemblyInfo.cs");
            string expectedVersion = "0.4.2";
            string expectedGameVer = "E.R.R";
            var task = new GetManifestInfoTask(manifestPath, assemblyPath);
            Assert.IsTrue(task.GetManifestInfo());
            Assert.AreEqual(expectedVersion, task.PluginVersion);
            Assert.AreEqual(expectedGameVer, task.GameVersion);
        }

        [TestMethod]
        public void MissingVersion()
        {
            string manifestPath = Path.Combine(TestFilePath, "BeatSync_manifest_MissingVersion.json");
            string assemblyPath = Path.Combine(TestFilePath, "BeatSync_AssemblyInfo.cs");
            string expectedVersion = "E.R.R";
            string expectedGameVer = "1.5.0";
            var task = new GetManifestInfoTask(manifestPath, assemblyPath);
            Assert.IsTrue(task.GetManifestInfo());
            Assert.AreEqual(expectedVersion, task.PluginVersion);
            Assert.AreEqual(expectedGameVer, task.GameVersion);
        }
    }
    /// <summary>
    /// </summary>
    public class GetManifestInfoTask
    {
        public TestLogger Log;
        public string ManifestFile;
        public string AssemblyFile;
        public string PluginVersion;
        public string AssemblyVersion;
        public string GameVersion;
        public GetManifestInfoTask(string manifestFile, string assemblyFile)
        {
            Log = new TestLogger();
            ManifestFile = manifestFile;
            AssemblyFile = assemblyFile;
        }

        /// <summary>
        /// Error IDs:
        /// BSMOD01: Error: AssemblyVersion does not match PluginVersion
        /// BSMOD02: Error: AssemblyVersion does not match AssemblyFileVersion
        /// BSMOD03: Warning: Could not parse AssemblyVersion
        /// BSMOD04: Error: PluginVersion not found in manifest.json
        /// BSMOD05: Error: GameVersion not found in manifest.json
        /// BSMOD06: Warning: Could not parse AssemblyFileVersion
        /// </summary>
        /// <returns></returns>
        public bool GetManifestInfo()
        {

            try
            {
                string manifestFile = "manifest.json";
                string assemblyFile = "Properties\\AssemblyInfo.cs";
                manifestFile = ManifestFile;
                assemblyFile = AssemblyFile;
                string manifest_gameVerStart = "\"gameVersion\"";
                string manifest_versionStart = "\"version\"";
                string manifest_gameVerLine = null;
                string manifest_versionLine = null;
                string startString = "[assembly: AssemblyVersion(\"";
                string secondStartString = "[assembly: AssemblyFileVersion(\"";
                string assemblyFileVersion = null;
                string firstLineStr = null;
                string endLineStr = null;
                bool badParse = false;
                int startLine = 1;
                int endLine = 0;
                int startColumn = 0;
                int endColumn = 0;
                if (!File.Exists(manifestFile))
                {
                    throw new FileNotFoundException("Could not find manifest: " + Path.GetFullPath(manifestFile));
                }
                if (!File.Exists(assemblyFile))
                {
                    throw new FileNotFoundException("Could not find AssemblyInfo: " + Path.GetFullPath(assemblyFile));
                }
                string line;
                using (StreamReader manifestStream = new StreamReader(manifestFile))
                {
                    while ((line = manifestStream.ReadLine()) != null && (manifest_versionLine == null || manifest_gameVerLine == null))
                    {
                        line = line.Trim();
                        if (line.StartsWith(manifest_gameVerStart))
                        {
                            manifest_gameVerLine = line;
                        }
                        else if (line.StartsWith(manifest_versionStart))
                        {
                            manifest_versionLine = line;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(manifest_versionLine))
                {
                    PluginVersion = manifest_versionLine.Substring(manifest_versionStart.Length).Replace(":", "").Replace("\"", "").TrimEnd(',').Trim();
                }
                else
                {
                    Log.LogError("Build", "BSMOD04", "", manifestFile, 0, 0, 0, 0, "PluginVersion not found in manifest.json");
                    PluginVersion = "E.R.R";
                }

                if (!string.IsNullOrEmpty(manifest_gameVerLine))
                {
                    GameVersion = manifest_gameVerLine.Substring(manifest_gameVerStart.Length).Replace(":", "").Replace("\"", "").TrimEnd(',').Trim();
                }
                else
                {
                    Log.LogError("Build", "BSMOD05", "", manifestFile, 0, 0, 0, 0, "GameVersion not found in manifest.json");
                    GameVersion = "E.R.R";
                }

                line = null;
                using (StreamReader assemblyStream = new StreamReader(assemblyFile))
                {
                    while ((line = assemblyStream.ReadLine()) != null)
                    {
                        if (line.Trim().StartsWith(startString))
                        {
                            firstLineStr = line;
                            break;
                        }
                        startLine++;
                        endLine = startLine + 1;
                    }
                    while ((line = assemblyStream.ReadLine()) != null)
                    {
                        if (line.Trim().StartsWith(secondStartString))
                        {
                            endLineStr = line;
                            break;
                        }
                        endLine++;
                    }
                }
                if (!string.IsNullOrEmpty(firstLineStr))
                {
                    startColumn = firstLineStr.IndexOf('"') + 1;
                    endColumn = firstLineStr.LastIndexOf('"');
                    if (startColumn > 0 && endColumn > 0)
                        AssemblyVersion = firstLineStr.Substring(startColumn, endColumn - startColumn);
                    else
                        badParse = true;
                }
                else
                    badParse = true;
                if (badParse)
                {
                    Log.LogError("Build", "BSMOD03", "", assemblyFile, 0, 0, 0, 0, "Unable to parse the AssemblyVersion from {0}", assemblyFile);
                    badParse = false;
                }

                if (PluginVersion != "E.R.R" && AssemblyVersion != PluginVersion)
                {
                    Log.LogError("Build", "BSMOD01", "", assemblyFile, startLine, startColumn + 1, startLine, endColumn + 1, "PluginVersion {0} in manifest.json does not match AssemblyVersion {1} in AssemblyInfo.cs", PluginVersion, AssemblyVersion, assemblyFile);
                    Log.LogMessage(MessageImportance.High, "PluginVersion {0} does not match AssemblyVersion {1}", PluginVersion, AssemblyVersion);
                }
                if (!string.IsNullOrEmpty(endLineStr))
                {
                    startColumn = endLineStr.IndexOf('"') + 1;
                    endColumn = endLineStr.LastIndexOf('"');
                    if (startColumn > 0 && endColumn > 0)
                    {
                        assemblyFileVersion = endLineStr.Substring(startColumn, endColumn - startColumn);
                        if (AssemblyVersion != assemblyFileVersion)
                            Log.LogWarning("Build", "BSMOD02", "", assemblyFile, endLine, startColumn + 1, endLine, endColumn + 1, "AssemblyVersion {0} does not match AssemblyFileVersion {1} in AssemblyInfo.cs", AssemblyVersion, assemblyFileVersion);

                    }
                    else
                    {
                        Log.LogWarning("Build", "BSMOD06", "", assemblyFile, 0, 0, 0, 0, "Unable to parse the AssemblyFileVersion from {0}", assemblyFile);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
                Log.LogErrorFromException(ex);
                return false;
            }

        }
    }

    public class TestLogger
    {
        public List<string> Messages = new List<string>();
        public void LogErrorFromException(Exception ex)
        {
            string message = $"{ex.Message}";
            Messages.Add(message);
            Console.Write(message);
        }
        public void LogMessage(MessageImportance importance, string message, params object[] messageArgs)
        {
            string msg = string.Format($"{importance.ToString()}: {message}", messageArgs);
            Messages.Add(msg);
            Console.WriteLine(msg);
        }
        public void LogError(string subcategory, string errorCode, string helpKeyword, string file,
            int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
        {
            string msg = string.Format($"ERROR: {subcategory}.{errorCode} | {file}({lineNumber}-{endLineNumber}:{columnNumber}-{endColumnNumber}): {message}", messageArgs);
            Messages.Add(msg);
            Console.WriteLine(msg);
        }
        public void LogWarning(string subcategory, string warningCode, string helpKeyword, string file,
            int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, params object[] messageArgs)
        {
            string msg = string.Format($"Warning: {subcategory}.{warningCode} | {file}({lineNumber}-{endLineNumber}:{columnNumber}-{endColumnNumber}): {message}", messageArgs);
            Messages.Add(msg);
            Console.WriteLine(msg);
        }
    }
}
