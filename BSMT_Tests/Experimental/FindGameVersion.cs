using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BSMT_Tests.Experimental
{
    /// <summary>
    /// From https://github.com/Assistant/ModAssistant
    /// </summary>
    [TestClass]
    public class FindGameVersion
    {
        public static readonly char[] IllegalCharacters = new char[]
            {
                '<', '>', ':', '/', '\\', '|', '?', '*', '"',
                '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007',
                '\u0008', '\u0009', '\u000a', '\u000b', '\u000c', '\u000d', '\u000e', '\u000d',
                '\u000f', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016',
                '\u0017', '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d', '\u001f',
            };
        public static readonly string GameDir = @"H:\SteamApps\SteamApps\common\Beat Saber";
        public static string GetVersion()
        {
            string filename = Path.Combine(GameDir, "Beat Saber_Data", "globalgamemanagers");
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] file = File.ReadAllBytes(filename);
                byte[] bytes = new byte[16];

                fs.Read(file, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                int index = Encoding.Default.GetString(file).IndexOf("public.app-category.games") + 136;

                Array.Copy(file, index, bytes, 0, 16);
                string version = Encoding.Default.GetString(bytes).Trim(IllegalCharacters);

                return version;
            }
        }

        [TestMethod]
        public void GameVersionTest()
        {
            string detectedVersion = GetVersion();
            Assert.AreEqual("1.6.0", detectedVersion);
        }
    }
}
