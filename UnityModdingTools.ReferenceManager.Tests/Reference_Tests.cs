using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModdingTools.Common;
using UnityModdingTools.Common.Models;

namespace UnityModdingTools.Projects.Tests
{
    [TestClass]
    public class Reference_Tests
    {
        [TestMethod]
        public void ParseFilter()
        {
            string gameFolder = @"D:\Steam\steamapps\common\Beat Saber";
            string pluginFolder = "Plugins";
            string libsFolder = "Libs";
            string managedFolder = Path.Combine("Beat Saber_Data", "Managed");
            string[] assemblyFolders = new[] { managedFolder, pluginFolder, libsFolder };
            List<string> assemblies = new List<string>();
            foreach (var dir in assemblyFolders)
            {
                string fullPath = Path.Combine(gameFolder, dir);
                assemblies.AddRange(Directory.EnumerateFiles(fullPath, "*.dll").Select(p => p.Replace(gameFolder + "\\", "@\"") + "\","));
            }
            Console.WriteLine(string.Join("\n", assemblies));

        }

        [TestMethod]
        public void RelativePathMatches()
        {
            string wantedRelativeDir = @"Beat Saber_Data\Managed\";
            string relativeDir = @"$(BeatSaberDir)\Beat Saber_Data\Managed\";
            string refName = "HMLib";
            string filename = refName + ".dll";
            string hintPath = Path.Combine(wantedRelativeDir, filename);
            ReferenceModel reference = new ReferenceModel("HMLib")
            {
                HintPath = hintPath
            };
            Assert.IsTrue(ReferenceFilterHandler.CheckPath(relativeDir, reference));

        }

        [TestMethod]
        public void RelativePathDoesntMatches()
        {
            string wantedRelativeDir = "Libs";
            string actualRelativeDir = @"$(BeatSaberDir)\Beat Saber_Data\Managed\";
            string refName = "HMLib";
            string filename = refName + ".dll";
            string hintPath = Path.Combine(actualRelativeDir, filename);
            ReferenceModel reference = new ReferenceModel("HMLib")
            {
                HintPath = hintPath
            };
            Assert.IsFalse(ReferenceFilterHandler.CheckPath(wantedRelativeDir, reference));

        }

        [TestMethod]
        public void OrFilterWithWildcardEnd()
        {
            string includeFilter = "Main;HM*;Zenject*;BeatmapCore;GameplayCore";
            ReferenceModel reference = new ReferenceModel("HMLib")
            {
                HintPath = @"Beat Saber_Data\Managed\HMLib.dll"
            };
            Assert.IsTrue(ReferenceFilterHandler.CheckFilter(includeFilter, reference));

        }

        [TestMethod]
        public void OrFilter()
        {
            string includeFilter = "Main;HM*;Zenject*;BeatmapCore;GameplayCore";
            (bool matches, string path)[] references = new[]
            {
                ( false, @"Beat Saber_Data\Managed\Accessibility.dll" ),
                (true, @"Beat Saber_Data\Managed\BeatmapCore.dll"),
                (false, @"Beat Saber_Data\Managed\BGNet.dll"),
                (true, @"Beat Saber_Data\Managed\HMLib.dll"),
                (true, @"Beat Saber_Data\Managed\HMLibAttributes.dll"),
                (true, @"Beat Saber_Data\Managed\HMRendering.dll"),
                (true, @"Beat Saber_Data\Managed\HMUI.dll"),
                (false, @"Beat Saber_Data\Managed\I18N.dll"),
                (false, @"Beat Saber_Data\Managed\Syst em.IO.Compression.FileSystem.dll"),
                (false, @"Beat Saber_Data\Managed\System.Net.Http.dll"),
                (false, @"Beat Saber_Data\Managed\System.Numerics.dll"),
                (false, @"Plugins\BeatSaverDownloader.dll"),
                (false, @"Plugins\BeatSaverVoting.dll"),
                (false, @"Plugins\BSML.dll"),
            };
            (bool matches, ReferenceModel refModel)[] refModels = references.Select(s =>
            {
                return
                (s.matches, new ReferenceModel(Path.GetFileNameWithoutExtension(s.path))
                {
                    HintPath = s.path
                }
                );
            }).ToArray();
            string? relativeDir = null;
            ReferenceFilterHandler handler = new ReferenceFilterHandler();
            foreach (var item in refModels)
            {
                Assert.AreEqual(item.matches, ReferenceFilterHandler.CheckFilter(includeFilter, item.refModel), $"Invalid match on {item.refModel.Name}");
            }

        }
    }
}
