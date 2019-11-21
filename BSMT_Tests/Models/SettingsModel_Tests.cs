using System;
using System.Reflection;
using BeatSaberModTemplates.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSMT_Tests.Models
{
    [TestClass]
    public class SettingsModel_Tests
    {
        private static SettingsModel AllChanged => new SettingsModel()
        {
            ChosenInstallPath = @"C:\Test\Testing",
            GenerateUserFileWithTemplate = true,
            GenerateUserFileOnExisting = true,
            SetManifestJsonDefaults = true,
            CopyToIPAPendingOnBuild = true,
            BuildReferenceType = BuildReferenceType.DirectoryJunctions,
            Manifest_Author = "AuthorTest",
            Manifest_Donation = "http://test.com/test",
            Manifest_AuthorEnabled = true,
            Manifest_DonationEnabled = true
        };

        [TestMethod]
        public void CopyConstructor_SettingsModel()
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

        [TestMethod]
        public void CopyConstructor_ReadOnlySettingsModel()
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

            var copy = new ReadOnlySettingsModel(original);

            Assert.AreEqual(original, copy);
        }

        [TestMethod]
        public void CopyConstructor_SettingsModel_IndividualTests()
        {
            var original = new SettingsModel();
            foreach (var prop in typeof(SettingsModel).GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(original, prop.Name);
                else if (prop.PropertyType == typeof(bool))
                    prop.SetValue(original, !(bool)prop.GetValue(original));
                else if (prop.PropertyType == typeof(BuildReferenceType))
                    prop.SetValue(original, BuildReferenceType.DirectoryJunctions);
                else
                    Assert.Fail($"Type {prop.PropertyType} is unhandled for {prop.Name}");
                var copy = new SettingsModel(original);
                Assert.AreEqual(original, copy, $"Failed after setting {prop.Name}");
            }
        }

        [TestMethod]
        public void CopyConstructor_ReadOnlySettingsModel_IndividualTests()
        {
            var original = new SettingsModel();
            foreach (var prop in typeof(SettingsModel).GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(original, prop.Name);
                else if (prop.PropertyType == typeof(bool))
                    prop.SetValue(original, !(bool)prop.GetValue(original));
                else if (prop.PropertyType == typeof(BuildReferenceType))
                    prop.SetValue(original, BuildReferenceType.DirectoryJunctions);
                else
                    Assert.Fail($"Type {prop.PropertyType} is unhandled for {prop.Name}");
                var copy = new ReadOnlySettingsModel(original);
                Assert.AreEqual(original, copy, $"Failed after setting {prop.Name}");
            }
        }

        [TestMethod]
        public void NotEqual_SettingsModel()
        {
            var original = new SettingsModel();
            foreach (var prop in typeof(SettingsModel).GetProperties())
            {
                var changed = new SettingsModel();
                Assert.AreEqual(original, changed);
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(changed, prop.Name);
                else if (prop.PropertyType == typeof(bool))
                    prop.SetValue(changed, !(bool)prop.GetValue(original));
                else if (prop.PropertyType == typeof(BuildReferenceType))
                    prop.SetValue(changed, BuildReferenceType.DirectoryJunctions);
                else
                    Assert.Fail($"Type {prop.PropertyType} is unhandled for {prop.Name}");
                Assert.AreNotEqual(original, changed, $"Failed after setting {prop.Name}");
            }
        }

        [TestMethod]
        public void NotEqual_ReadOnlySettingsModel()
        {
            var original = new SettingsModel();
            foreach (var prop in typeof(SettingsModel).GetProperties())
            {
                var changed = new SettingsModel();
                Assert.AreEqual(original, changed);
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(changed, prop.Name);
                else if (prop.PropertyType == typeof(bool))
                    prop.SetValue(changed, !(bool)prop.GetValue(original));
                else if (prop.PropertyType == typeof(BuildReferenceType))
                    prop.SetValue(changed, BuildReferenceType.DirectoryJunctions);
                else
                    Assert.Fail($"Type {prop.PropertyType} is unhandled for {prop.Name}");
                var readOnlyChanged = new ReadOnlySettingsModel(changed);
                Assert.AreEqual(changed, readOnlyChanged);
                Assert.AreNotEqual(original, changed, $"Failed after setting {prop.Name}");
                Assert.AreNotEqual(original, readOnlyChanged, $"Failed after setting {prop.Name}");
            }
        }
    }
}
