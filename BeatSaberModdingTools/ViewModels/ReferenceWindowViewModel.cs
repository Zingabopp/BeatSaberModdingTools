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

        public ObservableCollection<ReferenceItemViewModel> DesignExample => new ObservableCollection<ReferenceItemViewModel>()
        {
            new ReferenceItemViewModel(new ReferenceModel("TestInProject", null, @"C:\BeatSaber\Beat Saber_Data\Managed\TestInProject.dll"){  RelativeDirectory="Beat Saber_Data\\Managed", Version="1.1.1.0"}),
            new ReferenceItemViewModel()
        };
        public ObservableCollection<ReferenceItemViewModel> AvailableReferences { get; }
        public string ProjectFilePath { get; private set; }
        public string BeatSaberDir { get; private set; }
        public ReferenceWindowViewModel()
        {
            AvailableReferences = DesignExample;
            BeatSaberDir = @"C:\TestBeatSaberDir";
            //Refresh.Execute(null);
        }
        public ReferenceWindowViewModel(string projectFilePath, string beatSaberDir)
        {
            AvailableReferences = new ObservableCollection<ReferenceItemViewModel>();
            ProjectFilePath = projectFilePath;
            BeatSaberDir = BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath;
            Refresh.Execute(null);
        }

        public void RefreshData()
        {
            AvailableReferences.Clear();
            var refItems = BeatSaberTools.GetAvailableReferences(BeatSaberDir);
            var projRefs = XmlFunctions.GetReferences(ProjectFilePath);
            foreach (var item in refItems)
            {
                var projRefCount = projRefs.Where(r => r.Name == item.Name).Count();
                var refVM = new ReferenceItemViewModel(item);
                if (projRefCount > 0)
                {
                    refVM.IsInProject = true;
                    if (projRefCount > 1)
                        refVM.WarningStr = $"Project contains more than one reference to {item.Name}";
                }
                AvailableReferences.Add(refVM);
            }
        }

        #region Commands
        private RelayCommand _refresh;
        public RelayCommand Refresh
        {
            get
            {
                if (_refresh == null)
                    _refresh = new RelayCommand(() => RefreshData());
                return _refresh;
            }
        }

        #endregion
    }
}