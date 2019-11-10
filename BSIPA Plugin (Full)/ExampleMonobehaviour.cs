using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace $safeprojectname$
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class ExampleMonobehaviour : MonoBehaviour
    {
        /// <summary>
        /// Since we only want one of this particular script to exist at a time, keep a reference here.
        /// </summary>
        private static ExampleMonobehaviour instance;

        /// <summary>
        /// Coroutines are a way to simulate multithreading. 
        /// </summary>
        private IEnumerator ExampleCoroutine()
        {
            Logger.log?.Info($"{name}.ExampleCoroutine(): In ExampleCoroutine().");
            WaitForSeconds exampleWait = new WaitForSeconds(5); // Created here so we can reuse it in the loop.
            WaitUntil waitUntilTrue = new WaitUntil(() => Plugin.ExampleGameplayBoolSetting);
            while (true)
            {
                int count = 1;
                while (Plugin.ExampleGameplayBoolSetting)
                {
                    Logger.log?.Info($"{name}.ExampleCoroutine(): ExampleGamePlayBoolSetting is true, count is {count}, ExampleGameplayListSetting: {Plugin.ExampleGameplayListSetting}");
                    count++;
                    // yield return new WaitForSeconds(5); // Could do this, but it would create extra garbage to recreate the same object every time.
                    yield return exampleWait;
                }
                Logger.log?.Info($"{name}.ExampleCoroutine(): ExampleGamePlayBoolSetting is false, waiting until it's true again.");
                yield return waitUntilTrue; // Wait until Plugin.ExampleGamePlayBoolSetting is true again.
            }
        }

        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // We only want one of this particular script to exist at a time. If it already exists, destroy the old one.
            if (instance != null)
                GameObject.Destroy(instance.gameObject);
            instance = this;
        }
        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start() 
        {
            Logger.log?.Info($"{name}: In Start()");
            StartCoroutine(ExampleCoroutine());
            Logger.log?.Info($"{name}: Coroutine has been started.");
        }

        /// <summary>
        /// Called every frame if the script is enabled.
        /// </summary>
        private void Update() 
        { 
        
        }

        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate() 
        { 
        
        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {

        }

        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable() 
        {
        
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {

        }
        #endregion
    }
}
