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
        public ICollectionView ReferenceView;
        public ObservableCollection<ReferenceItemViewModel> DesignExample => new ObservableCollection<ReferenceItemViewModel>()
        {
            new ReferenceItemViewModel(new ReferenceModel("TestInProject", null, @"C:\BeatSaber\Beat Saber_Data\Managed\TestInProject.dll"){  RelativeDirectory="Beat Saber_Data\\Managed", Version="1.1.1.0"}, false),
            new ReferenceItemViewModel()
        };

        public ObservableCollection<ReferenceFilter> Filters { get; } = new ObservableCollection<ReferenceFilter>()
        {
            new ReferenceFilter("<all>", string.Empty, string.Empty),
            new ReferenceFilter("UnityEngine", Paths.Path_Managed, "UnityEngine"),
            new ReferenceFilter("System", Paths.Path_Managed, "System"),
            new ReferenceFilter("Libs", Paths.Path_Libs, string.Empty),
            new ReferenceFilter("Plugins", Paths.Path_Plugins, string.Empty)
        };

        private ReferenceFilter _selectedFilter;
        public ReferenceFilter SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter?.Name == value.Name) return;
                _selectedFilter = value;
                NotifyPropertyChanged();
                ReferenceView?.Refresh();
            }
        }

        public bool Filter(object item)
        {
            bool shown = true;
            if (item is ReferenceItemViewModel target)
            {

                if (!string.IsNullOrEmpty(SelectedFilter.Prefix) && !target.Name.StartsWith(SelectedFilter.Prefix))
                    shown = false;
                if (shown && !string.IsNullOrEmpty(SelectedFilter.RelativeDir) && target.RelativeDirectory != SelectedFilter.RelativeDir)
                    shown = false;
                return shown;
            }
            else
                shown = false;
            return shown;
        }

        public ObservableCollection<ReferenceItemViewModel> AvailableReferences { get; }
        public string ProjectFilePath { get; private set; }
        public string BeatSaberDir { get; private set; }
        public ReferenceWindowViewModel()
        {
            AvailableReferences = DesignExample;
            BeatSaberDir = @"C:\TestBeatSaberDir";
            SelectedFilter = Filters.First();
            //Refresh.Execute(null);
        }
        public ReferenceWindowViewModel(string projectFilePath, string beatSaberDir)
        {
            AvailableReferences = new ObservableCollection<ReferenceItemViewModel>();
            ProjectFilePath = projectFilePath;
            BeatSaberDir = BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath;
            SelectedFilter = Filters.First();
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

                ReferenceItemViewModel refVM = null;
                if (projRefCount > 0)
                {
                    refVM = new ReferenceItemViewModel(item, true);
                    refVM.IsInProject = true;
                    if (projRefCount > 1)
                        refVM.WarningStr = $"Project contains more than one reference to {item.Name}";
                }
                else
                    refVM = new ReferenceItemViewModel(item, false);
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

    public class ReferenceFilter
    {
        public string Name { get; }
        public string RelativeDir { get; }
        public string Prefix { get; }
        public ReferenceFilter(string name, string relativeDir, string prefix)
        {
            Name = name;
            RelativeDir = relativeDir;
            Prefix = prefix;
        }

        public Func<ReferenceItemViewModel, bool> IsMatch { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}