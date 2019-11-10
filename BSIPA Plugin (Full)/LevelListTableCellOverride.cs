using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using TMPro;

/// <summary>
/// See https://github.com/pardeike/Harmony/wiki for a full reference on Harmony.
/// </summary>
namespace $safeprojectname$.HarmonyPatches
{
    /// <summary>
    /// This is a patch of the method <see cref="LevelListTableCell.SetDataFromLevelAsync(IPreviewBeatmapLevel)"/>
    /// TODO: Remove this or replace it with your own.
    /// </summary>
    [HarmonyPatch(typeof(LevelListTableCell), "SetDataFromLevelAsync",
        new Type[] { // Specify the types of SetDataFromLevelAsync's parameters here.
        typeof(IPreviewBeatmapLevel)})]
    class LevelListTableCellOverride
    {
        /// <summary>
        /// Adds this plugin's name to the beginning of the author text in the song list view.
        /// </summary>
        /// <param name="__instance">The instance of <see cref="LevelListTableCell"/>, only here for example</param>
        /// <param name="level">The <see cref="IPreviewBeatmapLevel"/> that was passed to SetDataFromLevelAsync, only here for example.</param>
        /// <param name="____authorText">Actual name is '_authorText', added three _ to the beginning to reference it in the patch. Adding ref means we can change it.</param>
        [HarmonyAfter(new string[] { Plugin.SongCoreHarmonyId })] // SongCore patches the same method so we want to make sure our's is applied after.
        static void Postfix(LevelListTableCell __instance, ref IPreviewBeatmapLevel level, ref TextMeshProUGUI ____authorText)
        { 
            ____authorText.text = $"({Plugin.Name}) {____authorText.text}";
            ____authorText.ForceMeshUpdate();
            Logger.log.Warn($"Changed: {____authorText.text}");
        }
    }
}
