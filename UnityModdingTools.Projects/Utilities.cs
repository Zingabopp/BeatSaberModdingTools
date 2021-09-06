using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace UnityModdingTools.Projects
{
    public static class Utilities
    {
        public static readonly string ConditionSeparator = " && ";
        public static XDocument GenerateUserProject()
        {
            XNamespace xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
            XDocument? document = new XDocument(
                new XDeclaration("1.0", "utf-8", "Yes"),
                new XElement(xmlns + "Project",
                    new XAttribute("ToolsVersion", "Current"),
                    new XAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003")
                ));
            return document;
        }

        private static bool IncludeAll(ReferenceModel r) => true;

        public static ReferenceModel[] FindReferences(this XDocument xDoc, Func<ReferenceModel, bool>? filter = null, bool startedInProject = true)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc));
            XElement projElement = xDoc.FirstNode as XElement ?? throw new ArgumentException("xDoc doesn't have a root element?", nameof(xDoc));
            return projElement.FindReferences(filter, startedInProject);

        }

        public static ReferenceModel[] FindReferences(this XElement element, Func<ReferenceModel, bool>? filter = null, bool startedInProject = true)
        {
            if (filter == null)
                filter = IncludeAll;

            List<ReferenceModel> refs = new List<ReferenceModel>();
            List<string>? conditions = null;
            string? condition = element.Attribute("Condition")?.Value;
            if (condition != null)
            {
                conditions = new List<string>();
                conditions.Add(condition);
            }
            FindReferences(refs, element, conditions, filter, startedInProject);
            return refs.ToArray();
        }

        private static void FindReferences(List<ReferenceModel> refs, XElement element, List<string>? parentConditions, Func<ReferenceModel, bool> filter, bool startedInProject)
        {
            string? topCondition = element.Attribute("Condition")?.Value;
            if (topCondition != null)
            {
                if (parentConditions == null)
                    parentConditions = new List<string>();
                parentConditions.Add(topCondition);
            }
            foreach (XElement? e in element.Elements("Reference"))
            {
                string? name = e.Attribute("Include").Value;
                string? condition = e.Attribute("Condition")?.Value;
                string? hintPath = e.Element("HintPath")?.Value;
                string? privateStr = e.Element("Private")?.Value;
                CopyLocal copyLocal = CopyLocal.Undefined;
                if (privateStr != null)
                {
                    if (privateStr.Equals("True", StringComparison.OrdinalIgnoreCase))
                        copyLocal = CopyLocal.True;
                    else if (privateStr.Equals("False", StringComparison.OrdinalIgnoreCase))
                        copyLocal = CopyLocal.False;
                }
                string? refCondition = condition;
                if (parentConditions != null)
                {
                    refCondition = string.Join(ConditionSeparator, parentConditions);
                    if (condition != null)
                        refCondition = refCondition + ConditionSeparator + condition;
                }

                ReferenceModel refModel = new ReferenceModel(name)
                {
                    HintPath = hintPath,
                    Private = copyLocal,
                    Condition = refCondition,
                    StartedInProject = startedInProject
                };
                refs.Add(refModel);
            }
            foreach (XElement ig in element.Elements("ItemGroup"))
            {
                FindReferences(refs, ig, parentConditions, filter, startedInProject);
            }
        }

        public static XElement ToXML(this ReferenceModel model, XNamespace xNamespace)
        {
            XElement e = new XElement(xNamespace + "Reference",
                new XAttribute("Include", model.Name));
            if (!string.IsNullOrWhiteSpace(model.HintPath))
                e.Add(new XElement(xNamespace + "HintPath", model.HintPath));
            if (model.Private != CopyLocal.Undefined)
                e.Add(new XElement(xNamespace + "Private", model.Private == CopyLocal.True ? true : false));
            return e;
        }
    }
}
