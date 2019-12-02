using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeatSaberModdingTools.Utilities;
using static BeatSaberModdingTools.Utilities.Paths;
using System.Xml.XPath;

namespace BSMT_Tests.Experimental
{
    [TestClass]
    public class XmlReading
    {
        [TestMethod]
        public void ProjectParser()
        {
            var existingPath = @"Data\BSMLPractice.csproj";
            ProjectParser parser = new ProjectParser(existingPath);
            if (parser.TryParseProjectFile(out var results))
            {
                Assert.IsFalse(string.IsNullOrEmpty(results.BeatSaberDir));
                Assert.IsFalse(string.IsNullOrEmpty(results.ManifestFilePath));
                Assert.IsFalse(string.IsNullOrEmpty(results.PostBuildEvent));
                Assert.IsNotNull(results.Document);
                Assert.IsNotNull(results.MainPropertyGroupElement);
                Assert.IsNotNull(results.PostBuildEventElement);
                Assert.IsTrue(results.Targets.Count > 0);
                Assert.IsTrue(results.BuildTasks.Count > 0);
                Assert.IsTrue(results.References.Count > 0);
                Assert.IsTrue(results.References.TrueForAll(r => r.ParentGroup != null));
                Assert.IsTrue(results.References.Where(r => r.Name == "SongCore").Count() == 1);
                Console.WriteLine(results.MainPropertyGroupElement.ToString());
            }
            else
                Assert.Fail();
        }


        [TestMethod]
        public void ReadAll()
        {
            var existingPath = @"Data\BSMLPractice.csproj";
            XDocument doc = XDocument.Load(existingPath);
            doc.IterateThroughAllNodes((n, depth) =>
            {
                var padding = string.Empty.PadLeft(depth * 3, ' ');
                bool newLine = false;
                var printStr = string.Empty;
                bool needsClosing = false;
                if (n is XElement element)
                {
                    printStr += $"{padding}<{element.Name.LocalName}";
                    var attributes = element.Attributes().Select(a => $"{a.Name.LocalName}=\"{a.Value}\"");
                    printStr += $" {string.Join(" ", attributes)}>";
                    newLine = true;
                }
                if (n is XText text)
                    printStr += $"{padding}{text.Value}";
                if (n is XNode node && node.NodeType == XmlNodeType.EndElement)
                    printStr += $"{string.Empty.PadLeft((depth - 1) * 3, ' ')}{node.ToString()}";
                if (!newLine)
                    Console.WriteLine(printStr.Trim());
                else
                    Console.Write(printStr.TrimEnd());
            });
        }

        [TestMethod]
        public void GetAllReferences()
        {
            var existingPath = @"Data\BSMLPractice.csproj";
            var list = XmlFunctions.GetReferences(existingPath, true);
            foreach (var item in list)
            {
                string filePath = item.HintPath.Replace("$(BeatSaberDir)", @"H:\SteamApps\SteamApps\common\Beat Saber");
                if (File.Exists(filePath))
                {
                    var assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(filePath);
                }
                Console.WriteLine(item.ToString(30));
            }

            var otherList = BeatSaberTools.GetAvailableReferences(@"H:\SteamApps\SteamApps\common\Beat Saber");
            foreach (var item in otherList)
            {
                Console.WriteLine(item.ToString(30));
            }
            foreach (var item in list)
            {
                var check = otherList.Where(r => r.Name == item.Name).Single();
                if (item.HintPath.Contains("$(BeatSaberDir)"))
                    Assert.AreEqual(check.RelativeDirectory, item.RelativeDirectory);
            }

        }
    }
}
