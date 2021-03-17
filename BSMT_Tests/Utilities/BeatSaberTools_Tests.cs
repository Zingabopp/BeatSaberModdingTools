using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberModdingTools.Utilities;

namespace BSMT_Tests.Utilities
{
    [TestClass]
    public class BeatSaberTools_Tests
    {
        [TestMethod]
        public void FindBeatSaberDir()
        {
            var installs = BeatSaberTools.GetBeatSaberPathsFromRegistry();
        }
    }
}
