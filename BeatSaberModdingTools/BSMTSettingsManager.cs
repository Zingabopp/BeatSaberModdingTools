﻿using System;
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
        public ReadOnlySettingsModel CurrentSettings { get; private set; }

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
            if (CurrentSettings == null)
                UpdateCurrentSettings();
        }

        public void Store(ISettingsModel newSettings)
        {
            CurrentSettings = new ReadOnlySettingsModel(newSettings);
            Settings.ChosenInstallPath = CurrentSettings.ChosenInstallPath;
            Settings.GenerateUserFileWithTemplate = CurrentSettings.GenerateUserFileWithTemplate;
            Settings.GenerateUserFileOnExisting = CurrentSettings.GenerateUserFileOnExisting;
            Settings.SetManifestJsonDefaults = CurrentSettings.SetManifestJsonDefaults;
            Settings.CopyToIPAPendingOnBuild = CurrentSettings.CopyToIPAPendingOnBuild;
            Settings.BuildReferenceType = (byte)newSettings.BuildReferenceType;
            Settings.Manifest_Author = newSettings.Manifest_Author;
            Settings.Manifest_Donation = newSettings.Manifest_Donation;
            Settings.Manifest_AuthorEnabled = newSettings.Manifest_AuthorEnabled;
            Settings.Manifest_DonationEnabled = newSettings.Manifest_DonationEnabled;
            Settings.Save();
        }

        public void Reload()
        {
            Settings.Reload();
            UpdateCurrentSettings();
        }

        public void UpdateCurrentSettings()
        {
            try
            {
                CurrentSettings = new ReadOnlySettingsModel(Settings.ChosenInstallPath, Settings.GenerateUserFileWithTemplate, Settings.GenerateUserFileOnExisting,
                     Settings.SetManifestJsonDefaults, Settings.CopyToIPAPendingOnBuild, (BuildReferenceType)Settings.BuildReferenceType,
                     Settings.Manifest_Author, Settings.Manifest_Donation, Settings.Manifest_AuthorEnabled, Settings.Manifest_DonationEnabled);
            }
            catch (NullReferenceException)
            {
                CurrentSettings = new ReadOnlySettingsModel();
            }
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
        ReadOnlySettingsModel CurrentSettings { get; }
        bool IsDestroyed { get; }
        void Destroy();
        void Initialize();
        void Store(ISettingsModel settings);
        void Reload();
        void UpdateCurrentSettings();

    }
}