using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using UnityModdingTools.Common.Utilities;

namespace UnityModdingTools.Common.Tests.Utilities
{
    [TestClass]
    public class ReferencePathParser_Tests
    {
        private static string[] BSRelativeDirs => new string[] { "Beat Saber_Data\\Managed", "Plugins", "Libs" };
        private static string[] BSRelativeForwardSlashDirs => new string[] { "Beat Saber_Data\\Managed", "Plugins", "Libs" };

        [TestMethod]
        [DataTestMethod]
        [DataRow(TestOption.None, DisplayName = nameof(FilenameOnly))]
        [DataRow(TestOption.WithRelativeDir, DisplayName = nameof(FilenameOnly) + "_RelativeDir")]
        [DataRow(TestOption.NoExtension, DisplayName = nameof(FilenameOnly) + "_NoExtension")]
        [DataRow(TestOption.RD_NX, DisplayName = nameof(FilenameOnly) + "_RD_NX")]
        public void FilenameOnly(TestOption testOptions)
        {
            bool useRelativeDirs = testOptions.HasFlag(TestOption.WithRelativeDir);
            bool useForwardSlashes = testOptions.HasFlag(TestOption.ForwardSlashes);
            bool noExtension = testOptions.HasFlag(TestOption.NoExtension);

            string[]? relativeDirs = useRelativeDirs ? BSRelativeDirs : null;
            ReferencePathParser parser = new ReferencePathParser(relativeDirs);

            string? gamePath = null;
            string? relativePath = null;
            string? fullPath = null;
            string assemblyName = "BeatmapCore.Test";
            string? extension = noExtension ? null : ".dll";
            string assemblyPath = assemblyName + extension;

            if (!useRelativeDirs)
            {
                gamePath = fullPath;
                relativePath = null;
            }
            if (useForwardSlashes)
            {
                gamePath = gamePath?.Replace('\\', '/');
                relativePath = relativePath?.Replace('\\', '/');
                fullPath = fullPath?.Replace('\\', '/');
                assemblyPath = assemblyPath.Replace('\\', '/');
            }

            ParseResult result = DoMatch(parser, assemblyPath, testOptions);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(gamePath, result.GamePath?.TrimEnd('\\'));
            Assert.AreEqual(relativePath, result.RelativePath);
            Assert.AreEqual(assemblyName, result.AssemblyName);
            Assert.AreEqual(extension, result.Extension);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow(TestOption.None, DisplayName = nameof(FullPath))]
        [DataRow(TestOption.WithRelativeDir, DisplayName = nameof(FullPath) + "_RelativeDir")]
        [DataRow(TestOption.ForwardSlashes, DisplayName = nameof(FullPath) + "_ForwardSlashes")]
        [DataRow(TestOption.NoExtension, DisplayName = nameof(FullPath) + "_NoExtension")]
        [DataRow(TestOption.FS_RD, DisplayName = nameof(FullPath) + "_FS_RD")]
        [DataRow(TestOption.FS_NX, DisplayName = nameof(FullPath) + "_FS_NX")]
        [DataRow(TestOption.RD_NX, DisplayName = nameof(FullPath) + "_RD_NX")]
        public void FullPath(TestOption testOptions)
        {
            bool useRelativeDirs = testOptions.HasFlag(TestOption.WithRelativeDir);
            bool useForwardSlashes = testOptions.HasFlag(TestOption.ForwardSlashes);
            bool noExtension = testOptions.HasFlag(TestOption.NoExtension);

            string[]? relativeDirs = useRelativeDirs ? BSRelativeDirs : null;
            ReferencePathParser parser = new ReferencePathParser(relativeDirs);
            string? gamePath = @"I:\Steam\SteamApps\Common\Beat Saber";
            string? relativePath = @"Beat Saber_Data\Managed";
            string? fullPath = Path.Combine(gamePath, relativePath);
            string assemblyName = "BeatmapCore.Test";
            string? extension = noExtension ? null : ".dll";
            string assemblyPath = Path.Combine(fullPath, assemblyName + extension);

            if (!useRelativeDirs)
            {
                gamePath = fullPath;
                relativePath = null;
            }
            if (useForwardSlashes)
            {
                gamePath = gamePath?.Replace('\\', '/');
                relativePath = relativePath?.Replace('\\', '/');
                fullPath = fullPath?.Replace('\\', '/');
                assemblyPath = assemblyPath.Replace('\\', '/');
            }

            ParseResult result = DoMatch(parser, assemblyPath, testOptions);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(gamePath, result.GamePath?.TrimEnd('\\'));
            Assert.AreEqual(relativePath, result.RelativePath);
            Assert.AreEqual(assemblyName, result.AssemblyName);
            Assert.AreEqual(extension, result.Extension);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow(TestOption.None, DisplayName = nameof(RelativePath))]
        [DataRow(TestOption.WithRelativeDir, DisplayName = nameof(RelativePath) + "_RelativeDir")]
        [DataRow(TestOption.ForwardSlashes, DisplayName = nameof(RelativePath) + "_ForwardSlashes")]
        [DataRow(TestOption.NoExtension, DisplayName = nameof(RelativePath) + "_NoExtension")]
        [DataRow(TestOption.FS_RD, DisplayName = nameof(RelativePath) + "_FS_RD")]
        [DataRow(TestOption.FS_NX, DisplayName = nameof(RelativePath) + "_FS_NX")]
        [DataRow(TestOption.RD_NX, DisplayName = nameof(RelativePath) + "_RD_NX")]
        public void RelativePath(TestOption testOptions)
        {
            bool useRelativeDirs = testOptions.HasFlag(TestOption.WithRelativeDir);
            bool useForwardSlashes = testOptions.HasFlag(TestOption.ForwardSlashes);
            bool noExtension = testOptions.HasFlag(TestOption.NoExtension);

            string[]? relativeDirs = useRelativeDirs ? BSRelativeDirs : null;
            ReferencePathParser parser = new ReferencePathParser(relativeDirs);

            string? gamePath = @"$(BeatSaberDir)";
            string? relativePath = @"Beat Saber_Data\Managed";
            string? fullPath = Path.Combine(gamePath, relativePath);
            string assemblyName = "BeatmapCore.Test";
            string? extension = noExtension ? null : ".dll";
            string assemblyPath = Path.Combine(fullPath, assemblyName + extension);

            if (!useRelativeDirs)
            {
                gamePath = fullPath;
                relativePath = null;
            }
            if (useForwardSlashes)
            {
                gamePath = gamePath?.Replace('\\', '/');
                relativePath = relativePath?.Replace('\\', '/');
                fullPath = fullPath?.Replace('\\', '/');
                assemblyPath = assemblyPath.Replace('\\', '/');
            }

            ParseResult result = DoMatch(parser, assemblyPath, testOptions);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(gamePath, result.GamePath?.TrimEnd('\\'));
            Assert.AreEqual(relativePath, result.RelativePath);
            Assert.AreEqual(assemblyName, result.AssemblyName);
            Assert.AreEqual(extension, result.Extension);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow(null, DisplayName = "InvalidPath_NullString")]
        [DataRow("", DisplayName = "InvalidPath_EmptyString")]
        [DataRow("  ", DisplayName = "InvalidPath_WhitespaceString")]
        public void InvalidPath(string str)
        {
            ReferencePathParser parser = new ReferencePathParser();
            Assert.ThrowsException<ArgumentNullException>(() => parser.Parse(str));
        }

        [Flags]
        public enum TestOption
        {
            None = 0,
            WithRelativeDir = 1 << 0,
            ForwardSlashes = 1 << 1,
            NoExtension = 1 << 2,
            FS_RD = ForwardSlashes | WithRelativeDir,
            FS_NX = ForwardSlashes | NoExtension,
            RD_NX = WithRelativeDir | NoExtension

        }


        private ParseResult DoMatch(ReferencePathParser parser, string path, TestOption options)
        {
            ParseResult result = parser.Parse(path);

            bool useRelativeDirs = options.HasFlag(TestOption.WithRelativeDir);
            bool useForwardSlashes = options.HasFlag(TestOption.ForwardSlashes);
            bool noExtension = options.HasFlag(TestOption.NoExtension);

            Console.WriteLine($"Matching {(useRelativeDirs ? "with" : "without")} Relative Directories:\n {path}");
            if (result.Success)
            {
                Console.WriteLine($"Match: {result.Match}\n\nGamePath: {result.GamePath}\nRelativeDir: {result.RelativePath}\nAssemblyName: {result.AssemblyName}\nExtension: {result.Extension}\n\n");
            }
            else
                Console.WriteLine("No match\n\n");
            Console.WriteLine($"Used Pattern: '{parser.Pattern}'");
            return result;
        }
    }
}
