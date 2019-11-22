using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BeatSaberModTemplates.Utilities.Paths;

namespace BeatSaberModTemplates.Utilities
{
    public static class XmlGenerator
    {
        public static XDocument GenerateReferencePaths(string beatSaberDir)
        {

            var referencePaths = new string[] { Path_Managed, Path_Libs, Path_Plugins }.Select(p => Path.Combine(beatSaberDir, p)).ToList();
            XNamespace xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(xmlns + "Project",
                    new XAttribute("ToolsVersion", "Current"),
                    new XAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003"),
                        new XElement(xmlns + "PropertyGroup",
                        new XElement(xmlns + "ReferencePath", string.Join(";", referencePaths)))
                ));
            return document;
        }

        public static XDocument GenerateReferencePaths(string beatSaberDir, string outputFilePath)
        {
            var doc = GenerateReferencePaths(beatSaberDir);
            doc.Save(outputFilePath, SaveOptions.OmitDuplicateNamespaces);
            return doc;
        }
    }
}
