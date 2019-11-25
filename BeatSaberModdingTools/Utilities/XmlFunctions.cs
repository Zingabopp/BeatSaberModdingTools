using BeatSaberModdingTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BeatSaberModdingTools.Utilities.Paths;

namespace BeatSaberModdingTools.Utilities
{
    public static class XmlFunctions
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

        public static List<ReferenceModel> GetReferences(string xmlFilePath, bool externalOnly = false, Func<string> match = null)
        {
            var referenceList = new List<ReferenceModel>();
            var doc = XDocument.Load(xmlFilePath);
            doc.IterateThroughAllNodes((n, depth) =>
            {
                if (n is XElement element)
                {
                    if (element.Name.LocalName == "Reference")
                    {

                        string refName = element.Attributes()?.Where(a => a.Name.LocalName == "Include").FirstOrDefault()?.Value;
                        string hintPath = element.Elements().Where(e => e.Name.LocalName == "HintPath").FirstOrDefault()?.Value ?? string.Empty;
                        if (!string.IsNullOrEmpty(refName))
                        {
                            if (!string.IsNullOrEmpty(hintPath) || !externalOnly)
                            {
                                referenceList.Add(new ReferenceModel(refName, n.Parent, hintPath));
                            }
                        }
                    }
                }
            });

            return referenceList;
        }



    }


}
