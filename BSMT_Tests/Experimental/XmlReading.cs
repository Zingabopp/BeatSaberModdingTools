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
        }
    }
}
