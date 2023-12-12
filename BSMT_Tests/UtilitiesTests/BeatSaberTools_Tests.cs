using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberModdingTools.Utilities;

namespace BSMT_Tests.UtilitiesTests
{
    [TestClass]
    public class BeatSaberTools_Tests
    {
        [TestMethod]
        public void FindGameDirectory()
        {
            var expectedSteamInstalls = 1;
            var expectedOculusInstalls = 0;
            var expectedBSManagerInstalls = 1;
            var expectedManualInstalls = 0;
            var installs = BeatSaberTools.GetBeatSaberPathsFromRegistry();
            Assert.AreEqual(expectedSteamInstalls, installs.Where((install) => install.InstallType == BeatSaberModdingTools.Models.InstallType.Steam).Count());
            Assert.AreEqual(expectedOculusInstalls, installs.Where((install) => install.InstallType == BeatSaberModdingTools.Models.InstallType.Oculus).Count());
            Assert.AreEqual(expectedBSManagerInstalls, installs.Where((install) => install.InstallType == BeatSaberModdingTools.Models.InstallType.BSManager).Count());
            Assert.AreEqual(expectedManualInstalls, installs.Where((install) => install.InstallType == BeatSaberModdingTools.Models.InstallType.Manual).Count());
        }
    }
}
