using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityModdingTools.Common.Configuration
{
    public class ReferenceManagerFilterSetting : ConfigBase
    {
        private string _name;

        [JsonProperty(nameof(Name))]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                RaiseConfigChanged();
                NotifyPropertyChanged();
            }
        }


        private string? _relativeDirectory;
        /// <summary>
        /// Path to included assemblies starting from the game folder.
        /// </summary>
        [JsonProperty(nameof(RelativeDirectory))]
        public string? RelativeDirectory
        {
            get { return _relativeDirectory; }
            set
            {
                if (_relativeDirectory == value)
                    return;
                _relativeDirectory = value;
                RaiseConfigChanged();
                NotifyPropertyChanged();
            }
        }

        private string? _includeFilter;
        /// <summary>
        /// Filter pattern of files to include.
        /// </summary>
        [JsonProperty(nameof(IncludeFilter))]
        public string? IncludeFilter
        {
            get { return _includeFilter; }
            set
            {
                if (_includeFilter == value)
                    return;
                _includeFilter = value;
                RaiseConfigChanged();
                NotifyPropertyChanged();
            }
        }


        [JsonConstructor]
        public ReferenceManagerFilterSetting([JsonProperty(nameof(Name))] string name)
        {
            _name = name;
        }
    }
}
