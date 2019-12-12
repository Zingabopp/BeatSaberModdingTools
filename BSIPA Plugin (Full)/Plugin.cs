using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine;
using Harmony;
using IPALogger = IPA.Logging.Logger;
using System.Reflection;

namespace $safeprojectname$
{
    public class Plugin : IBeatSaberPlugin
    {
        // TODO: Change YourGitHub to the name of your GitHub account, or use the form "com.company.project.product"
        public const string HarmonyId = "com.github.YourGitHub.$safeprojectname$";
        public const string SongCoreHarmonyId = "com.kyle1413.BeatSaber.SongCore";
        internal static HarmonyInstance harmony;
        internal static string Name => "$projectname$";
        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;

        #region Gameplay Settings
        internal static bool ExampleGameplayBoolSetting { get; set; }
        public static float ExampleGameplayListSetting { get; set; }
        #endregion


        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            Logger.log.Debug("Logger initialised.");

            configProvider = cfgProvider;

            config = configProvider.MakeLink<PluginConfig>((p, v) =>
            {
                // Build new config file if it doesn't exist or RegenerateConfig is true
                if (v.Value == null || v.Value.RegenerateConfig)
                {
                    Logger.log.Debug("Regenerating PluginConfig");
                    p.Store(v.Value = new PluginConfig()
                    {
                        // Set your default settings here.
                        RegenerateConfig = false,
                        ExampleBoolSetting = false,
                        ExampleIntSetting = 5,
                        ExampleColorSetting = UnityEngine.Color.blue.ToFloatAry(),
                        ExampleTextSegment = 0,
                        ExampleStringSetting = "example",
                        ExampleSliderSetting = 2,
                        ExampleListSetting = 3f
                    });
                }
                config = v;
            });
            harmony = HarmonyInstance.Create(HarmonyId);
        }

        public void OnApplicationStart()
        {
            ExampleGameplayBoolSetting = true;
            Logger.log.Debug("OnApplicationStart");
            BS_Utils.Utilities.BSEvents.menuSceneLoadedFresh += MenuLoadedFresh;
            ApplyHarmonyPatches();
        }

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

        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");
        }

        /// <summary>
        /// Runs at a fixed intervalue, generally used for physics calculations. 
        /// </summary>
        public void OnFixedUpdate()
        {

        }

        /// <summary>
        /// This is called every frame.
        /// </summary>
        public void OnUpdate()
        {

        }

        /// <summary>
        /// Called when the active scene is changed.
        /// </summary>
        /// <param name="prevScene">The scene you are transitioning from.</param>
        /// <param name="nextScene">The scene you are transitioning to.</param>
        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "HealthWarning")
            {
                var reflectionUtilExample = new GameObject($"{Name}.ReflectionUtilExample").AddComponent<ReflectionUtilExample>();
            }
            if (nextScene.name == "MenuCore")
            {
                var exampleGameObject = new GameObject($"{Name}.ExampleMonobehaviour").AddComponent<ExampleMonobehaviour>();
            }
            if (nextScene.name == "GameCore")
            {

            }
        }

        /// <summary>
        /// Called when BSEvents.menuSceneLoadedFresh is triggered. UI creation is in here instead of
        /// OnSceneLoaded because some settings won't work otherwise.
        /// </summary>
        public void MenuLoadedFresh()
        {
            Plugin.configProvider.Store(Plugin.config.Value);
            Logger.log.Debug("Creating plugin's UI");
            UI.$safeprojectname$_UI.CreateUI();
        }
        /// <summary>
        /// Called when the a scene's assets are loaded.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sceneMode"></param>
        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {



        }

        public void OnSceneUnloaded(Scene scene)
        {

        }
    }
}
