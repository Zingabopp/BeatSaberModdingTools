using System;
using System.Collections.Generic;
using System.Linq;
using IPA.Utilities; // ReflectionUtil lives here.
using UnityEngine;
using UnityEngine.UI;

namespace $safeprojectname$
{
    public class ReflectionUtilExample : MonoBehaviour
    {
        public static ReflectionUtilExample instance;
        private void Awake()
        {
            if (instance != null)
                GameObject.DestroyImmediate(this);
            instance = this;
        }

        private void Start()
        {
            StartCoroutine(AutoClickContinue());
        }

        private IEnumerator<WaitForSeconds> AutoClickContinue()
        {
            var waitTime = new WaitForSeconds(.5f);
            var clickWait = new WaitForSeconds(5);
            HealthWarningViewController healthController;
            do
            {
                healthController = GameObject.FindObjectsOfType<HealthWarningViewController>().FirstOrDefault();
                if (healthController == null)
                    yield return waitTime;
                else
                {
                    Button continueButton = null;
                    try
                    {
                        continueButton = healthController.GetPrivateField<Button>("_continueButton");
                    }
                    catch (Exception ex)
                    {
                        Logger.log?.Error(ex);
                        break;
                    }
                    if (continueButton != null)
                    {
                        Logger.log?.Info($"Button found! Waiting 5 seconds before simulating a click.");
                        yield return clickWait;
                        continueButton.onClick.Invoke();
                    }
                }
            } while (healthController == null);
        }

    }
}
