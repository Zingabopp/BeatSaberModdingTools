using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace $safeprojectname$
{
    public class $safeprojectname$ : MelonMod
    {
        public static class BuildInfo
        {
            public const string Name = "$projectname$";  // Name of the Mod.  (MUST BE SET)
            public const string Author = "$username$"; // Author of the Mod.  (Set as null if none)
            public const string Company = null; // Company that made the Mod.  (Set as null if none)
            public const string Version = "0.0.1"; // Version of the Mod.  (MUST BE SET)
            public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
        }


        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                MelonLogger.Log("You just pressed T");
            }
        }
    }
}
