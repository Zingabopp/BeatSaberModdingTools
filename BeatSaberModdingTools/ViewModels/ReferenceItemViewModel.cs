using BeatSaberModdingTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.ViewModels
{
    public class ReferenceItemViewModel : ViewModelBase
    {
        private ReferenceModel _reference;
        public ReferenceModel Reference
        {
            get { return _reference; }
            set
            {
                if (_reference == value) return;
                _reference = value;
                NotifyPropertyChanged(string.Empty);
            }
        }

        // For designer
        internal ReferenceItemViewModel()
        {
            Reference = new ReferenceModel("TestName")
            {
                HintPath = @"$(BeatSaberDir)\Plugins\Test.dll",
                RelativeDirectory = "Plugins",
                Version = "1.0.1.0"
            };
        }

        public ReferenceItemViewModel(ReferenceModel model, bool isInProject)
        {
            Reference = model;
            StartedInProject = isInProject;
            IsInProject = isInProject;
        }

        public string Name => Reference?.Name;
        public string Version => Reference?.Version;
        public string RelativeDirectory => Reference?.RelativeDirectory;

        public bool StartedInProject { get; }

        private bool _isInProject;
        public bool IsInProject
        {
            get { return _isInProject; }
            set
            {
                if (_isInProject == value) return;
                _isInProject = value;
                NotifyPropertyChanged();
            }
        }

        public string HintPath
        {
            get { return Reference.HintPath; }
            set
            {
                if (Reference?.HintPath == value || Reference == null) return;
                Reference.HintPath = value;
                NotifyPropertyChanged();
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


    }
}
