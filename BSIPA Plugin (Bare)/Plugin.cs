using IPA;
using IPALogger = IPA.Logging.Logger;

namespace $safeprojectname$
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger log { get; set; }

        [Init]
        public Plugin(IPALogger logger)
        {
            Instance = this;
            log = logger;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            
        }

    }
}
