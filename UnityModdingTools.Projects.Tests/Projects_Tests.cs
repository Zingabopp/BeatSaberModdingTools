using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityModdingTools.Abstractions;
using UnityModdingTools.Common.Models;

namespace UnityModdingTools.Projects.Tests
{
    [TestClass]
    public class Projects_Tests
    {
        public int ChangedCalled = 0;

        [TestMethod]
        public void ActualProject()
        {
            string asmName = GetType().Namespace;
            string path = asmName + ".Data.sdk_project.csproj.test";

            ReferenceModel refModel = new ReferenceModel("AddTest")
            {
                HintPath = "C:\\Test\\AddTest.dll",
                Private = CopyLocal.False
            };

            string[] thing = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            using System.IO.Stream rs = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            XDocument proj = XDocument.Load(rs);
            XElement projElement = (XElement)proj.FirstNode;
            XElement refGroup = projElement.Elements(Names.ItemGroup).Where(e =>
                e.Attribute("Condition") == null &&
                e.Elements(Names.Reference).Any()).FirstOrDefault();
            Assert.IsNotNull(refGroup);

            XNamespace ns = projElement.Name.Namespace;

            refGroup.AddFirst(refModel.ToXML(ns));

            proj.Save("actualProject.csproj");
        }

        [TestMethod]
        public void FindReference()
        {
            string asmName = GetType().Namespace;
            string path = asmName + ".Data.sdk_project.csproj.test";

            ReferenceModel refModel = new ReferenceModel("AddTest")
            {
                HintPath = "C:\\Test\\AddTest.dll",
                Private = CopyLocal.False
            };

            string[] thing = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            using System.IO.Stream rs = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            XDocument proj = XDocument.Load(rs);
            XElement projElement = (XElement)proj.FirstNode;
            var refs = proj.FindReferences();
        }


        [TestMethod]
        public void UserProj()
        {
            XDocument userProj = Utilities.GenerateUserProject();
            userProj.Changed += UserProj_Changed;
            XElement projElement = (XElement)userProj.FirstNode;
            Assert.AreEqual("Project", projElement.Name.LocalName);
            XAttribute attribute = projElement.Attribute("ToolsVersion");
            Assert.AreEqual("Current", attribute.Value);
            attribute.Value = "NotCurrent";
            Assert.AreEqual(1, ChangedCalled);
            XNamespace ns = projElement.Name.Namespace;
            projElement.Add(
                new XElement(ns + "ItemGroup",
                    new XElement(ns + "Reference",
                        new XAttribute("Include", "Test"),
                        new XElement(ns + "HintPath", "C:\\Test\\Test.dll"),
                        new XElement(ns + "Private", "False"))));
            userProj.Save("test.xml");
        }

        private void UserProj_Changed(object sender, System.Xml.Linq.XObjectChangeEventArgs e)
        {
            ChangedCalled++;
            Console.WriteLine($"ChangedCalled ({sender.GetType().Name}:{e.ObjectChange}): {ChangedCalled}");
        }
    }
}
