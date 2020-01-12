using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace $rootnamespace$
{
    /// <summary>
    /// This patches ClassToPatch.MethodToPatch(Parameter1Type arg1, Parameter2Type arg2)
    /// </summary>
    [HarmonyPatch(typeof(ClassToPatch), "MethodToPatch",
        new Type[] { // List the Types of the method's parameters.
        typeof(Parameter1Type),
        typeof(Parameter2Type)})]
    public class $safeitemrootname$
    {
        /// <summary>
        /// This code is run before the original code in MethodToPatch is run.
        /// </summary>
        /// <param name="__instance">The instance of ClassToPatch</param>
        /// <param name="arg1">The Parameter1Type arg1 that was passed to MethodToPatch</param>
        /// <param name="____privateFieldInClassToPatch">Reference to the private field in ClassToPatch named '_privateFieldInClassToPatch', 
        ///     added three _ to the beginning to reference it in the patch. Adding ref means we can change it.</param>
        [HarmonyAfter(new string[] { "Another.mods.HarmonyID" })] // If another mod patches this method, apply this patch after the other mod's.
        static bool Prefix(ClassToPatch __instance, ref Parameter1Type arg1, ref string ____privateFieldInClassToPatch)
        {
            arg1 = new Parameter1Type("ChangedValue"); // This will change arg1 for anything in MethodToPatch after this.
            ____privateFieldInClassToPatch = "private field changed";
            bool stopRunningMethodToPatch;
            if (stopRunningMethodToPatch)
                return false; // If you return false in prefix, the rest of the code in MethodToPatch is skipped.
            return true;
        }

        /// <summary>
        /// This code is run after the original code in MethodToPatch is run.
        /// </summary>
        /// <param name="__instance">The instance of ClassToPatch</param>
        /// <param name="arg1">The Parameter1Type arg1 that was passed to MethodToPatch</param>
        /// <param name="____privateFieldInClassToPatch">Reference to the private field in ClassToPatch named '_privateFieldInClassToPatch', 
        ///     added three _ to the beginning to reference it in the patch. Adding ref means we can change it.</param>
        static void Postfix(ClassToPatch __instance, ref Parameter1Type arg1, ref string ____privateFieldInClassToPatch)
        {
            
        }
    }
}