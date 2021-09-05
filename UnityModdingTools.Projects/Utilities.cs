using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace UnityModdingTools.Projects
{
    public static class Utilities
    {
        public static XDocument GenerateUserProject()
        {
            XNamespace xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(xmlns + "Project",
                    new XAttribute("ToolsVersion", "Current"),
                    new XAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003")
                ));
            return document;
        }
    }
}
