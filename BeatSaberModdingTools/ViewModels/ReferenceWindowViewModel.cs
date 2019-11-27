using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.ViewModels
{
    public class ReferenceWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ReferenceItemViewModel> AvailableReferences { get; }
        public string BeatSaberDir { get; private set; }
        public ReferenceWindowViewModel()
        {
            AvailableReferences = new ObservableCollection<ReferenceItemViewModel>();
            BeatSaberDir = BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath;
            Refresh.Execute(null);
        }

        public void RefreshData()
        {
            AvailableReferences.Clear();
            var refItems = BeatSaberTools.GetAvailableReferences(BeatSaberDir);
            var projRefs = new List<ReferenceModel>();
            foreach (var item in refItems)
            {
                var projRefCount = projRefs.Where(r => r.Name == item.Name).Count();
                var refVM = new ReferenceItemViewModel(item);
                if (projRefCount > 1)
                    refVM.WarningStr = $"Project contains more than one reference to {item.Name}";
                AvailableReferences.Add(refVM);
            }
        }

        #region Commands
        private RelayCommand _refresh;
        public RelayCommand Refresh
        {
            get
            {
                if(_refresh == null)
                    _refresh = new RelayCommand(() => RefreshData());
                return _refresh;
            }
        }

        #endregion
    }
}