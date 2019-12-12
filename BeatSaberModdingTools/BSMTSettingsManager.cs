using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberModdingTools.Models;
using Microsoft.VisualStudio.Shell;
using BeatSaberModdingTools.Utilities;

namespace BeatSaberModdingTools
{
    public class BSMTSettingsManager : IBSMTSettingsManager
    {
        private Properties.BeatSaberModdingToolsSettings Settings => Properties.BeatSaberModdingToolsSettings.Default;
        private ISettingsModel _currentSettings;
        public ISettingsModel CurrentSettings {
            get
            {
                if (_currentSettings == null)
                    _currentSettings = new ActiveSettings();
                return _currentSettings;
            }
            private set
            {
                _currentSettings = value;
            }
        }

        public bool IsDestroyed { get; protected set; }

        public static IBSMTSettingsManager Instance { get; private set; }
        public static void UseDefaultManager()
        {
            SetManager(new BSMTSettingsManager());
        }
        public static void SetManager(IBSMTSettingsManager manager)
        {
            if (Instance != null)
                Instance.Destroy();
            Instance = manager;
            manager.Initialize();
        }
        public void Destroy() { IsDestroyed = true; }
        public void Initialize()
        {
            if (_currentSettings == null)
            {
                _currentSettings = new ActiveSettings();
                NotifySettingsChanged();
            }
        }

        public void Store(ISettingsModel newSettings)
        {
            Settings.ChosenInstallPath = newSettings.ChosenInstallPath;
            Settings.GenerateUserFileWithTemplate = newSettings.GenerateUserFileWithTemplate;
            Settings.GenerateUserFileOnExisting = newSettings.GenerateUserFileOnExisting;
            Settings.SetManifestJsonDefaults = newSettings.SetManifestJsonDefaults;
            Settings.CopyToIPAPendingOnBuild = newSettings.CopyToIPAPendingOnBuild;
            Settings.BuildReferenceType = (byte)newSettings.BuildReferenceType;
            Settings.Manifest_Author = newSettings.Manifest_Author;
            Settings.Manifest_Donation = newSettings.Manifest_Donation;
            Settings.Manifest_AuthorEnabled = newSettings.Manifest_AuthorEnabled;
            Settings.Manifest_DonationEnabled = newSettings.Manifest_DonationEnabled;
            Settings.Save();
            NotifySettingsChanged();
        }

        public void Reload()
        {
            Settings.Reload();
            NotifySettingsChanged();
        }

        public void NotifySettingsChanged()
        {
            foreach (var subscriber in subscribers)
            {
                if (subscriber.IsAlive)
                    subscriber.Execute();
                else
                {
                    UnsubscribeExecuteOnChange(subscriber);
                }
            }
        }

        public static void SubscribeExecuteOnChange(WeakAction action)
        {
            if (!subscribers.Contains(action))
                subscribers.Add(action);
        }

        public static bool UnsubscribeExecuteOnChange(WeakAction action)
        {
            return subscribers.Remove(action);
        }

        private static List<WeakAction> subscribers = new List<WeakAction>();
    }

    public interface IBSMTSettingsManager
    {
        ISettingsModel CurrentSettings { get; }
        bool IsDestroyed { get; }
        void Destroy();
        void Initialize();
        void Store(ISettingsModel settings);
        void Reload();
        void NotifySettingsChanged();

    }
}
