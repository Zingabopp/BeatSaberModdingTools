using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IPA;
using IPA.Config;
using Harmony;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace $safeprojectname$
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        // TODO: If using Harmony, uncomment and change YourGitHub to the name of your GitHub account, or use the form "com.company.project.product"
        // public const string HarmonyId = "com.github.YourGitHub.$safeprojectname$";
        // internal static HarmonyInstance harmony => HarmonyInstance.Create(HarmonyId);

        internal static string Name => "$projectname$";

        [Init]
        public void Init(IPALogger logger)
        {
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        //[Init]
        //public void InitWithConfig(Config conf)
        //{
        //    Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
        //    Logger.log.Debug("Config loaded");
        //}
        #endregion


        #region IDisablable

        /// <summary>
        /// Called when the plugin is enabled (including when the game starts if the plugin is enabled).
        /// </summary>
        [OnEnable]
        public void OnEnable()
        {
            //ApplyHarmonyPatches();
        }

        /// <summary>
        /// Called when the plugin is disabled and on Beat Saber quit. It is important to clean up any Harmony patches, GameObjects, and Monobehaviours here.
        /// The game should be left in a state as if the plugin was never started.
        /// </summary>
        [OnDisable]
        public void OnDisable()
        {
            //RemoveHarmonyPatches();
        }
        #endregion

        // Uncomment the methods in this section if using Harmony
        #region Harmony
    /*
        /// <summary>
        /// Attempts to apply all the Harmony patches in this assembly.
        /// </summary>
        public static void ApplyHarmonyPatches()
        {
            try
            {
                Logger.log.Debug("Applying Harmony patches.");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Logger.log.Critical("Error applying Harmony patches: " + ex.Message);
                Logger.log.Debug(ex);
            }
        }

        /// <summary>
        /// Attempts to remove all the Harmony patches that used our HarmonyId.
        /// </summary>
        public static void RemoveHarmonyPatches()
        {
            try
            {
                // Removes all patches with this HarmonyId
                harmony.UnpatchAll(HarmonyId);
            }
            catch (Exception ex)
            {
                Logger.log.Critical("Error removing Harmony patches: " + ex.Message);
                Logger.log.Debug(ex);
            }
        }
    */
    #endregion
}
}
