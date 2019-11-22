﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BeatSaberModTemplates.Utilities.Paths;

namespace BSMT_Tests.Experimental
{
    [TestClass]
    public class XMLCreation
    {
        public const string BeatSaberDir = @"H:\SteamApps\SteamApps\common\Beat Saber";
        [TestMethod]
        public void TestMethod1()
        {
            var referencePaths = new string[]{ Path_Managed, Path_Libs, Path_Plugins }.Select(p => Path.Combine(BeatSaberDir, p)).ToList();
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
    }
}