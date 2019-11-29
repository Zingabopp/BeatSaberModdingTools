using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLangProj;

namespace BeatSaberModdingTools.ViewModels
{
    public class ReferenceWindowViewModel : ViewModelBase
    {
        public ICollectionView ReferenceView;
        private VSProject _project;
        public ObservableCollection<ReferenceItemViewModel> DesignExample => new ObservableCollection<ReferenceItemViewModel>()
        {
            new ReferenceItemViewModel(this, new ReferenceModel( "TestInProject", null, @"C:\BeatSaber\Beat Saber_Data\Managed\TestInProject.dll"){  RelativeDirectory="Beat Saber_Data\\Managed", Version="1.1.1.0"}, false),
            new ReferenceItemViewModel()
        };

        public ObservableCollection<ReferenceFilter> Filters { get; } = new ObservableCollection<ReferenceFilter>()
        {
            new ReferenceFilter("<all>", string.Empty, string.Empty),
            new ReferenceFilter("System", Paths.Path_Managed, "System."),
            new ReferenceFilter("Unity", Paths.Path_Managed, "Unity."),
            new ReferenceFilter("UnityEngine", Paths.Path_Managed, "UnityEngine."),
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
        public ReferenceWindowViewModel(string projectFilePath, VSProject project, string beatSaberDir)
        {
            AvailableReferences = new ObservableCollection<ReferenceItemViewModel>();
            ProjectFilePath = projectFilePath;
            BeatSaberDir = beatSaberDir;
            _project = project;
            SelectedFilter = Filters.First();
            Refresh.Execute(null);
        }

        public void RefreshData()
        {
            AvailableReferences.Clear();
            var refItems = BeatSaberTools.GetAvailableReferences(BeatSaberDir);
            var buildProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectFilePath).First();
            var references = buildProject.Items.Where(obj => obj.ItemType == "Reference").ToList();
            var projRefs = new List<ReferenceModel>();
            foreach (var item in references)
            {
                var refModel = new ReferenceModel(item.UnevaluatedInclude);
                if (item.HasMetadata("HintPath"))
                    refModel.HintPath = item.Metadata.Where(m => m.Name == "HintPath").First().UnevaluatedValue;
                projRefs.Add(refModel);
            }
            foreach (var item in refItems)
            {
                var projRefCount = projRefs.Where(r => r.Name == item.Name).Count();

                ReferenceItemViewModel refVM = null;
                if (projRefCount > 0)
                {
                    refVM = new ReferenceItemViewModel(this, item, true);
                    refVM.IsInProject = true;
                    if (projRefCount > 1)
                        refVM.WarningStr = $"Project contains more than one reference to {item.Name}";
                }
                else
                    refVM = new ReferenceItemViewModel(this, item, false);
                AvailableReferences.Add(refVM);
            }
        }

        public void CheckChangedReferences()
        {
            var changedRefs = AvailableReferences.Where(r => r.StartedInProject != r.IsInProject).ToList();
            ReferencesChanged = AvailableReferences.Any(r => r.StartedInProject != r.IsInProject);
        }

        public void UpdateReferences()
        {
            var changedRefs = AvailableReferences.Where(r => r.StartedInProject != r.IsInProject).ToList();
            var removedRefs = changedRefs.Where(r => !r.IsInProject).ToList();
            foreach (var item in removedRefs)
            {
                var reference = _project.References.Find(item.Name);
                reference.Remove();
                item.StartedInProject = false;
            }
            var addedRefs = changedRefs.Where(r => r.IsInProject).ToList();
            foreach (var item in addedRefs)
            {
                var refPath = item.HintPath.Replace(BeatSaberDir, "$(BeatSaberDir)");
                var reference = _project.References.Add(item.HintPath);
                reference.CopyLocal = false;

                item.StartedInProject = true;
            }
            var buildProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectFilePath).First();
            foreach (var item in addedRefs)
            {
                var needsHint = buildProject.Items.Where(obj => obj.ItemType == "Reference" && obj.EvaluatedInclude == item.Name).First();
                needsHint.SetMetadataValue("HintPath", $"$(BeatSaberDir)\\{item.RelativeDirectory}\\{item.Name}.dll");
            }
            CheckChangedReferences();
        }

        private bool _referencesChanged;
        public bool ReferencesChanged
        {
            get { return _referencesChanged; }
            private set
            {
                if (_referencesChanged == value) return;
                _referencesChanged = value;
                NotifyPropertyChanged();
                ApplyChanges.RaiseCanExecuteChanged();
            }
        }

        private bool _saveInProgress;
        public bool SaveInProgress
        {
            get { return _saveInProgress; }
            private set
            {
                if (_saveInProgress == value) return;
                _saveInProgress = value;
                NotifyPropertyChanged();
            }
        }

        public async Task SaveProjectAsync()
        {
            SaveInProgress = true;
            await Microsoft.VisualStudio.Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            _project.Project.Save();
            SaveInProgress = false;
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

        private RelayCommand _applyChanges;
        public RelayCommand ApplyChanges
        {
            get
            {
                if (_applyChanges == null)
                    _applyChanges = new RelayCommand(() => UpdateReferences(), () =>
                    {
                        if (!SaveInProgress)
                            if (ReferencesChanged)
                                return true;
                        return false;
                    });
                return _applyChanges;
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