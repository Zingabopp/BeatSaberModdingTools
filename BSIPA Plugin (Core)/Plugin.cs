using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace $safeprojectname$
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin : IBeatSaberPlugin
    {
        internal static string Name => "$projectname$";

        [Init]
        public void Init(IPALogger logger)
        {
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
        }

        #region BSIPA Config
        // Uncomment to use BSIPA's config
        //[Init]
        //public void Init(IPALogger logger, Config conf)
        //{
        //    Logger.log = logger;
        //    Logger.log.Debug("Logger initialized.");
        //    PluginConfig.Instance = conf.Generated<PluginConfig>();
        //    Logger.log.Debug("Config loaded");
        //}
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Logger.log.Debug("OnApplicationStart");

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");

        }
    }
}
