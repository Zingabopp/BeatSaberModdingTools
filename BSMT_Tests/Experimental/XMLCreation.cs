using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BeatSaberModdingTools.Utilities.Paths;
using System.Xml.XPath;

namespace BSMT_Tests.Experimental
{
    [TestClass]
    public class XMLCreation
    {
        public const string BeatSaberDir = @"H:\SteamApps\SteamApps\common\Beat Saber";
        [TestMethod]
        public void WriteAndRead()
        {
            var referencePaths = new string[] { Path_Managed, Path_Libs, Path_Plugins }.Select(p => Path.Combine(BeatSaberDir, p)).ToList();
            XNamespace xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
            var thing2 = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(xmlns + "Project",
                    new XAttribute("ToolsVersion", "Current"),
                    new XAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003"),
                        new XElement("PropertyGroup",
                        new XElement("ReferencePath", string.Join(";", referencePaths)))
                ));

            Console.WriteLine("Test");
            Console.WriteLine(thing2.ToString(SaveOptions.OmitDuplicateNamespaces));
            var project = from p in thing2.Descendants()
                          where p.Name.LocalName == "Project"
                          select p;
            Assert.IsNotNull(project);
            var propGroup = from p in thing2.Descendants()
                            where p.Name.LocalName == "PropertyGroup"
                            select p;
            Assert.IsNotNull(propGroup);
            var refPaths = from p in thing2.Descendants()
                           where p.Name.LocalName == "ReferencePath"
                           select p;
            Assert.IsNotNull(refPaths);
            var paths = refPaths.First().Value;
            thing2.Save(@"Output.xml", SaveOptions.OmitDuplicateNamespaces);
        }

        [TestMethod]
        public void AppendExisting()
        {
            var existingPath = @"Output\ExistingUserFile\BeatSaberDir.csproj.user";
            Assert.IsTrue(File.Exists(existingPath));
            var doc = XDocument.Load(existingPath);
            var nameSpace = doc.Root.GetDefaultNamespace();
            var project = doc.Element("Project");
            if (TryGetFirstElement(doc, "PropertyGroup", out var propGroup))
            {
                propGroup = FindFirstElement(doc, "PropertyGroup");
                if (!TryGetFirstElement(propGroup, "ReferencePaths", out _))
                    propGroup.Add(new XElement("ReferencePaths", "test;test;test"));
                else
                    Assert.Fail("Shouldn't have found ReferencePaths");
                Assert.IsNotNull(project);
                Assert.IsNotNull(propGroup);
                if (!TryGetFirstElement(propGroup, "ReferencePaths", out _))
                    Assert.Fail("Should have found ReferencePaths");
                doc.Save(existingPath + ".xml");
            }
            else
                Assert.Fail($"PropertyGroup not found in {existingPath}");

        }

        public static bool TryGetFirstElement(XContainer node, string localName, out XElement element)
        {
            element = FindFirstElement(node, localName);
            if (element != null)
                return true;
            return false;
        }

        public static XElement FindFirstElement(XContainer node, string localName)
        {
            var nodeElements = node?.Elements();
            if (nodeElements == null && nodeElements.Count() == 0)
                return null;
            foreach (var element in nodeElements)
            {
                if (element.Name.LocalName == localName)
                    return element;
            }
            foreach (var element in nodeElements)
            {
                var foundElement = FindFirstElement(element, localName);
                if (foundElement != null)
                    return foundElement;
            }
            return null;
        }

    }
}
