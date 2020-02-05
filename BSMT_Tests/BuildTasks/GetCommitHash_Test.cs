using Microsoft.Build.Framework;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_Tests.BuildTasks
{
    [TestClass]
    class GetCommitHash_Test
    {
        [TestMethod]
        public void GetCommitHashTest()
        {
            GetCommitHash testTask = new GetCommitHash();
            testTask.ProjectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Data", "BuildTaskTests"));
            Console.WriteLine(testTask.ProjectDir);
            Assert.IsTrue(testTask.Execute());
            Console.WriteLine(testTask.CommitShortHash);
            Assert.AreEqual("3b9c39e", testTask.CommitShortHash);
        }
    }


    public class GetCommitHash
    {
        public string CommitShortHash = "local";
        public string ProjectDir;
        TestLogger Log = new TestLogger();
        public bool Execute()
        {
            try
            {
                ProjectDir = Path.GetFullPath(ProjectDir);
                Process process = new Process();
                string arg = "rev-parse HEAD";
                process.StartInfo = new ProcessStartInfo("gitas", arg);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = ProjectDir;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                var outText = process.StandardOutput.ReadToEnd();
                if (outText.Length >= 7)
                    CommitShortHash = outText.Substring(0, 7);
                else
                    Log.LogMessage(MessageImportance.High, "Unable to retrieve current commit hash.");
                return true;
            }
            catch (Win32Exception ex)
            {
                string gitPath = Path.GetFullPath(Path.Combine(ProjectDir, ".git"));
                string headPath = Path.Combine(gitPath, "HEAD");
                string headContents = null;
                if (File.Exists(headPath))
                    headContents = File.ReadAllText(headPath);
                else
                {
                    gitPath = Path.GetFullPath(Path.Combine(ProjectDir, "..", ".git"));
                    headPath = Path.Combine(gitPath, "HEAD");
                    if (File.Exists(headPath))
                        headContents = File.ReadAllText(headPath);
                }
                headPath = null;
                if (!string.IsNullOrEmpty(headContents) && headContents.StartsWith("ref:"))
                    headPath = Path.Combine(gitPath, headContents.Replace("ref:", "").Trim());
                if(File.Exists(headPath))
                {
                    headContents = File.ReadAllText(headPath);
                    if(headContents.Length >= 7)
                        CommitShortHash = headContents.Substring(0, 7);
                }
                else
                    Log.LogMessage(MessageImportance.High, "   git command not found, unable to retrieve current commit hash.");
                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return true;
            }
        }
    }
}
