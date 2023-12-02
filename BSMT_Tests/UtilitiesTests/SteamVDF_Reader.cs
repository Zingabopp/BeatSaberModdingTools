using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberModdingTools.Utilities;

namespace BSMT_Tests.UtilitiesTests
{
    [TestClass]
    public class SteamVDF_Reader
    {
        [TestMethod]
        public void ReadVdf()
        {
            string path = Path.Combine("Data", "config.vdf");
            int expectedResults = 4;
            string[] libraryPaths = BeatSaberTools.LibrariesFromVdf(path);
            Assert.AreEqual(expectedResults, libraryPaths.Length);
            foreach (var libPath in libraryPaths)
            {
                Console.WriteLine(libPath);
            }
        }
    }
}
