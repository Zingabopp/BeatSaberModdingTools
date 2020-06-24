using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BeatSaberModdingTools.Models
{
    public class ReferenceModel : ObservableModel
    {
        private string name;
        private string fullInclude;
        private string hintPath;
        private string version;
        private string relativeDirectory;
        private bool includeStripped;
        private bool isInProject;

        public bool IsModified
        {
            get
            {
                return PreviousName != Name
                    || PreviousFullInclude != FullInclude
                    || PreviousHintPath != HintPath
                    || PreviousIncludeStripped != IncludeStripped
                    || PreviousIsInProject != IsInProject;
            }
        }



        public string PreviousName { get; protected set; }
        public string PreviousFullInclude { get; protected set; }
        public string PreviousHintPath { get; protected set; }
        public bool PreviousIncludeStripped { get; protected set; }
        public bool PreviousIsInProject { get; protected set; }

        public string Name
        {
            get => name;
            set
            {
                if (Name == value)
                    return;
                name = value;
                NotifyTrackedPropertyChanged();
            }
        }
        public string FullInclude
        {
            get => fullInclude;
            set
            {
                if (FullInclude == value)
                    return;
                fullInclude = value;
                NotifyTrackedPropertyChanged();
            }
        }
        public string HintPath
        {
            get => hintPath;
            set
            {
                if (HintPath == value)
                    return;
                hintPath = value;
                NotifyTrackedPropertyChanged();
            }
        }
        public XElement ParentGroup { get; set; }
        public string Version
        {
            get => version;
            set
            {
                if (Version == value)
                    return;
                version = value;
                NotifyPropertyChanged();
            }
        }
        public string RelativeDirectory
        {
            get => relativeDirectory;
            set
            {
                if (RelativeDirectory == value)
                    return;
                relativeDirectory = value;
                NotifyPropertyChanged();
            }
        }
        public bool IncludeStripped
        {
            get => includeStripped;
            set
            {
                if (IncludeStripped == value)
                    return;
                includeStripped = value;
                NotifyTrackedPropertyChanged();
            }
        }
        public bool IsInProject
        {
            get { return isInProject; }
            set
            {
                if (isInProject == value) return;
                isInProject = value;
                NotifyTrackedPropertyChanged();
            }
        }

        private string _errorStr;
        public string ErrorStr
        {
            get { return _errorStr; }
            set
            {
                if (_errorStr == value) return;
                _errorStr = value;
                NotifyPropertyChanged();
            }
        }

        private string _warningStr;
        public string WarningStr
        {
            get { return _warningStr; }
            set
            {
                if (_warningStr == value) return;
                _warningStr = value;
                NotifyPropertyChanged();
            }
        }

        public void CloneFrom(ReferenceModel other)
        {
            Name = other.Name;
            FullInclude = other.FullInclude;
            if (!string.IsNullOrEmpty(other.HintPath))
                HintPath = other.HintPath;
            if (!string.IsNullOrEmpty(other.Version))
                Version = other.Version;
            if (!string.IsNullOrEmpty(other.RelativeDirectory))
                RelativeDirectory = other.RelativeDirectory;
            IncludeStripped = other.IncludeStripped;
            IsInProject = other.IsInProject;
            ErrorStr = other.ErrorStr;
            WarningStr = other.WarningStr;
            if (!other.IsModified)
                ResetModified();
        }

        // For designer
        internal ReferenceModel()
        {
            Name = "Test.Reference";
            HintPath = @"$(BeatSaberDir)\Plugins\Test.dll";
            RelativeDirectory = "Plugins";
            Version = "1.0.1.0";
            ResetModified();
        }

        public ReferenceModel(string name)
        {
            FullInclude = name;
            if (name.Contains(","))
                Name = name.Substring(0, name.IndexOf(",")).Trim();
            else
                Name = name;
            ResetModified();
        }
        public ReferenceModel(string name, XElement parentGroup, bool includeStripped, string hintPath = "")
            : this(name)
        {
            ParentGroup = parentGroup;
            IncludeStripped = includeStripped;
            HintPath = hintPath;
            ResetModified();
        }

        public void ResetModified()
        {
            bool previousIsModified = IsModified;
            PreviousFullInclude = FullInclude;
            PreviousName = Name;
            PreviousIncludeStripped = IncludeStripped;
            PreviousHintPath = HintPath;
            PreviousIsInProject = IsInProject;
            if (IsModified != previousIsModified)
                NotifyPropertyChanged(nameof(IsModified));
        }

        public string ToString(int padding)
        {
            string retVal = Name;
            if (!string.IsNullOrEmpty(HintPath))
                retVal = retVal.PadRight(padding) + " | " + HintPath;
            return retVal;
        }

        public override string ToString()
        {
            return ToString(0);
        }
        protected void NotifyTrackedPropertyChanged([CallerMemberName] String propertyName = "")
        {
            NotifyPropertyChanged(propertyName);
            NotifyPropertyChanged(nameof(IsModified));
        }
    }
}
