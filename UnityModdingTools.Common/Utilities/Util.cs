using System.Diagnostics;
using System.IO;
using UnityModdingTools.Common.Models;

namespace UnityModdingTools.Common.Utilities
{
    public static class Util
    {
        public static ReferenceModel CreateReferenceFromFile(string fileName)
        {
            //var assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(fileName);
            //var assemblyName = assembly.GetName();
            string? version = null;
            if (File.Exists(fileName))
                version = FileVersionInfo.GetVersionInfo(fileName).FileVersion;
            // TODO: Try/Catch with logging.
            ReferenceModel refItem = new ReferenceModel(Path.GetFileNameWithoutExtension(fileName))
            {
                Version = version,
                HintPath = fileName
            };

            return refItem;
        }
    }
}
