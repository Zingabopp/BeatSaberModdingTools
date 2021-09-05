using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityModdingTools.Abstractions;

namespace UnityModdingTools.Projects.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public int ChangedCalled = 0;

        [TestMethod]
        public void ActualProject()
        {
            string asmName = GetType().Namespace;
            string path = asmName + ".Data.sdk_project.csproj.test";
            var thing = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var proj = XDocument.Load(rs);
            var projElement = proj.FirstNode as XElement;
            var refGroup = projElement.Elements(Names.ItemGroup).Where(e =>
                e.Attribute("Condition") == null &&
                e.Elements(Names.Reference).Any()).FirstOrDefault();
            Assert.IsNotNull(refGroup);

            var ns = projElement.Name.Namespace;
            refGroup.AddFirst(new XElement(ns + "Reference",
                    new XAttribute("Include", "AddTest"),
                    new XElement(ns + "HintPath", "C:\\Test\\AddTest.dll"),
                    new XElement(ns + "Private", "False")));
            
            proj.Save("actualProject.csproj");
        }


        [TestMethod]
        public void UserProj()
        {
            var userProj = Utilities.GenerateUserProject();
            userProj.Changed += UserProj_Changed;
            var projElement = userProj.FirstNode as XElement;
            Assert.AreEqual("Project", projElement.Name.LocalName);
            var attribute = projElement.Attribute("ToolsVersion");
            Assert.AreEqual("Current", attribute.Value);
            attribute.Value = "NotCurrent";
            Assert.AreEqual(1, ChangedCalled);
            var ns = projElement.Name.Namespace;
            projElement.Add(new XElement(ns + "ItemGroup",
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
