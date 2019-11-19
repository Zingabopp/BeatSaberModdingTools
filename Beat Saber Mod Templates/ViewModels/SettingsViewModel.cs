using BeatSaberModTemplates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModTemplates.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            
        }

        public SettingsViewModel(SettingsModel existingSettings)
        {
            _previousSettings = existingSettings;
            _currentSettings = existingSettings.Clone();
        }



        private SettingsModel _previousSettings;
        private SettingsModel _currentSettings;

        public SettingsModel CurrentSettings
        {
            get { return _currentSettings; }
            set
            {
                if (_currentSettings == value)
                    return;
                _currentSettings = value;
                NotifyPropertyChanged();
            }
        }
    }
}
