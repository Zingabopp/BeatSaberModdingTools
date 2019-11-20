using System;
using BeatSaberModTemplates.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSMT_Tests.Models
{
    [TestClass]
    public class SettingsModel_Tests
    {
        [TestMethod]
        public void Constructor_StillEquatable()
        {
            var expectedInstallPath = @"C:\Test\Testing";
            var GenUserWithTemplate = true;
            var GenUserOnExisting = true;
            var setManifestDefaults = true;
            var copyOnBuild = true;
            var expectedBRType = BuildReferenceType.DirectoryJunctions;
            var manifestAuthor = "AuthorTest";
            var manifestDonation = "http://test.com/test";
            var authorEnabled = true;
            var donationEnabled = true;
            var original = new SettingsModel()
            {
                ChosenInstallPath = expectedInstallPath,
                GenerateUserFileWithTemplate = GenUserWithTemplate,
                GenerateUserFileOnExisting = GenUserOnExisting,
                SetManifestJsonDefaults = setManifestDefaults,
                CopyToIPAPendingOnBuild = copyOnBuild,
                BuildReferenceType = expectedBRType,
                Manifest_Author = manifestAuthor,
                Manifest_Donation = manifestDonation,
                Manifest_AuthorEnabled = authorEnabled,
                Manifest_DonationEnabled = donationEnabled
            };

            var copy = new SettingsModel(original);

            Assert.AreEqual(original, copy);
        }
    }
}
