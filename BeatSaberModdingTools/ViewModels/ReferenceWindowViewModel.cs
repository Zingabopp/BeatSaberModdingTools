using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VSLangProj;

namespace BeatSaberModdingTools.ViewModels
{
    public class ReferenceWindowViewModel : ViewModelBase
    {
        public ICollectionView ReferenceView;
        private VSProject _project;
        private string _projectName { get; }
        private string _windowTitle;
        public string WindowTitle
        {
            get
            {
                if (_windowTitle == null)
                    _windowTitle = $"{_projectName} - Beat Saber Reference Manager";
                return _windowTitle;
            }
        }
        public ObservableCollection<ReferenceItemViewModel> DesignExample => new ObservableCollection<ReferenceItemViewModel>()
        {
            new ReferenceItemViewModel(this, new ReferenceModel( "TestInProject", null, @"C:\BeatSaber\Beat Saber_Data\Managed\TestInProject.dll"){  RelativeDirectory="Beat Saber_Data\\Managed", Version="1.1.1.0"}, false),
            new ReferenceItemViewModel()
        };

        public ObservableCollection<ReferenceFilter> Filters { get; } = new ObservableCollection<ReferenceFilter>()
        {
            new ReferenceFilter("<all>", string.Empty, string.Empty),
            new ReferenceFilter("Game", Paths.Path_Managed, new Func<ReferenceItemViewModel, bool>(t => t != null ? t.Name == "Main" || t.Name.StartsWith("HM") : false)),
            new ReferenceFilter("System", Paths.Path_Managed, "System."),
            new ReferenceFilter("Unity", Paths.Path_Managed, "Unity."),
            new ReferenceFilter("UnityEngine", Paths.Path_Managed, "UnityEngine."),
            new ReferenceFilter("Libs", Paths.Path_Libs, string.Empty),
            new ReferenceFilter("Plugins", Paths.Path_Plugins, string.Empty),
            new ReferenceFilter("IPA", Paths.Path_Managed, "IPA.")
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
            if (item is ReferenceItemViewModel target && SelectedFilter.IsValid)
            {

                if (!SelectedFilter.IsMatch(target))
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
            _projectName = "DesignerTest";
            BeatSaberDir = @"C:\TestBeatSaberDir";
            SelectedFilter = Filters.First();
            //Refresh.Execute(null);
        }
        public ReferenceWindowViewModel(string projectFilePath, string projectName, VSProject project, string beatSaberDir)
        {
            AvailableReferences = new ObservableCollection<ReferenceItemViewModel>();
            ProjectFilePath = projectFilePath;
            BeatSaberDir = beatSaberDir;
            _project = project;
            _projectName = projectName;
            SelectedFilter = Filters.First();
            Refresh.Execute(null);
        }

        public void RefreshData()
        {
            AvailableReferences.Clear();
            var refItems = BeatSaberTools.GetAvailableReferences(BeatSaberDir);
            var buildProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectFilePath).First();
            var references = buildProject.Items.Where(obj => obj.ItemType == "Reference").ToArray();
            var projRefs = new List<ReferenceModel>();
            foreach (var item in references)
            {
                var refModel = new ReferenceModel(item.UnevaluatedInclude);
                if (item.HasMetadata("HintPath"))
                    refModel.HintPath = item.Metadata.Where(m => m.Name == "HintPath").First().UnevaluatedValue;
                projRefs.Add(refModel);
            }
            foreach (var item in refItems.ToArray())
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
            var changedRefs = AvailableReferences.Where(r => r.StartedInProject != r.IsInProject).ToArray();
            var removedRefs = changedRefs.Where(r => !r.IsInProject).ToArray();
            foreach (var item in removedRefs)
            {
                var reference = _project.References.Find(item.Name);
                reference.Remove();
                item.StartedInProject = false;
            }
            var addedRefs = changedRefs.Where(r => r.IsInProject).ToArray();
            foreach (var item in addedRefs)
            {
                //var refPath = item.HintPath.Replace(BeatSaberDir, "$(BeatSaberDir)");
                if (_project.References.Find(item.Name) == null)
                {
                    Reference reference = _project.References.Add(item.HintPath);
                    reference.CopyLocal = false;
                    item.StartedInProject = true;
                }
                else
                {
                    item.StartedInProject = true;
                    item.IsInProject = true;
                }
            }

            var buildProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectFilePath).First();
            foreach (var item in addedRefs)
            {
                var newRef = buildProject.Items.Where(obj => obj.ItemType == "Reference" && GetSimpleReferenceName(obj.EvaluatedInclude) == item.Name).FirstOrDefault();
                KeyValuePair<string, string>[] meta = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("HintPath", $"$(BeatSaberDir)\\{item.RelativeDirectory}\\{item.Name}.dll"),
                    new KeyValuePair<string, string>("Private", "False"),
                    new KeyValuePair<string, string>("SpecificVersion", "False")
                };
                //if (newRef == null)
                //    newRef = buildProject.AddItem("Reference", item.Name).FirstOrDefault();
                if (newRef != null)
                {
                    foreach (var pair in meta)
                    {
                        newRef.SetMetadataValue(pair.Key, pair.Value);
                    }
                }
            }
            buildProject.MarkDirty();
            //buildProject.ReevaluateIfNecessary();
            //buildProject.Save();
            CheckChangedReferences();
        }

        private static string GetSimpleReferenceName(string fullInclude)
        {
            if (fullInclude.Contains(","))
                return fullInclude.Substring(0, fullInclude.IndexOf(",")).Trim();
            else
                return fullInclude;
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
        protected string Prefix { get; }
        public bool IsValid { get; }
        public Func<ReferenceItemViewModel, bool> IsMatch { get; protected set; }

        public ReferenceFilter(string name, string relativeDir, string prefix)
        {
            Name = name;
            RelativeDir = relativeDir;
            Prefix = prefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                IsMatch = new Func<ReferenceItemViewModel, bool>(t => t?.Name.StartsWith(prefix) ?? false);
            }
            else
                IsMatch = new Func<ReferenceItemViewModel, bool>(t => true);
            IsValid = true;
        }

        public ReferenceFilter(string name, string relativeDir, Func<ReferenceItemViewModel, bool> predicate)
        {
            Name = name;
            RelativeDir = relativeDir;
            IsValid = predicate != null;
            IsMatch = predicate;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}