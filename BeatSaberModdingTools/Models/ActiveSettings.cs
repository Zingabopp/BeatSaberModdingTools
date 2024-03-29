﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.Models
{
    public class ActiveSettings : SettingsBase
    {
        private Properties.BeatSaberModdingToolsSettings Settings => Properties.BeatSaberModdingToolsSettings.Default;
        public override string ChosenInstallPath
        {
            get { return Settings.ChosenInstallPath; }
            set { Settings.ChosenInstallPath = value; }
        }

        public override bool GenerateUserFileWithTemplate
        {
            get { return Settings.GenerateUserFileWithTemplate; }
            set { Settings.GenerateUserFileWithTemplate = value; }
        }

        public override bool GenerateUserFileOnExisting
        {
            get { return Settings.GenerateUserFileOnExisting; }
            set { Settings.GenerateUserFileOnExisting = value; }
        }

        public override bool SetManifestJsonDefaults
        {
            get { return Settings.SetManifestJsonDefaults; }
            set { Settings.SetManifestJsonDefaults = value; }
        }

        public override bool CopyToIPAPendingOnBuild
        {
            get { return Settings.CopyToIPAPendingOnBuild; }
            set { Settings.CopyToIPAPendingOnBuild = value; }
        }

        public override BuildReferenceType BuildReferenceType
        {
            get { return (BuildReferenceType)Settings.BuildReferenceType; }
            set { Settings.BuildReferenceType = (byte)value; }
        }

        public override string Manifest_Author
        {
            get { return Settings.Manifest_Author; }
            set { Settings.Manifest_Author = value; }
        }

        public override string Manifest_Donation
        {
            get { return Settings.Manifest_Donation; }
            set { Settings.Manifest_Donation = value; }
        }

        public override bool Manifest_AuthorEnabled
        {
            get { return Settings.Manifest_AuthorEnabled; }
            set { Settings.Manifest_AuthorEnabled = value; }
        }

        public override bool Manifest_DonationEnabled
        {
            get { return Settings.Manifest_DonationEnabled; }
            set { Settings.Manifest_DonationEnabled = value; }
        }

        public override bool Equals(object other)
        {
            if (other is ISettingsModel settings)
                return Equals(settings);
            else
                return false;
        }
    }
}
