using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace $safeprojectname$
{
    public class Plugin : IBeatSaberPlugin
    {
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
            Logger.log.Debug("Logger initialied.");

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
        }

        public void OnApplicationStart()
        {
            ExampleGameplayBoolSetting = true;
            Logger.log.Debug("OnApplicationStart");
            CustomUI.Utilities.BSEvents.menuSceneLoadedFresh += MenuLoadedFresh;

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
            if (nextScene.name == "MenuCore")
            {
                var exampleGameObject = new GameObject($"{Name}.ExampleMonobehaviour").AddComponent<ExampleMonobehaviour>();
            }
        }

        /// <summary>
        /// Called when BSEvents.menuSceneLoadedFresh is triggered. UI creation is in here instead of
        /// OnSceneLoaded because some settings won't work otherwise.
        /// </summary>
        public void MenuLoadedFresh()
        {
            {
                Logger.log.Debug("Creating plugin's UI");
                UI.$safeprojectname$_UI.CreateUI();
            }
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
