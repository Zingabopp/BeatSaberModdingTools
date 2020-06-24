using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
        public ObservableCollection<ReferenceModel> DesignExample
        {
            get
            {
                ReferenceModel testModel = new ReferenceModel("TestInProject", null, true, @"C:\BeatSaber\Beat Saber_Data\Managed\TestInProject.dll") { RelativeDirectory = "Beat Saber_Data\\Managed", Version = "1.1.1.0" };
                testModel.HintPath = @"C:\BeatSaber\Beat Saber_Data\Changed\TestInProject.dll";
                return new ObservableCollection<ReferenceModel>()
                        {
                            testModel,
                            new ReferenceModel()
                        };
            }
        }

        public ObservableCollection<ReferenceFilter> Filters { get; } = new ObservableCollection<ReferenceFilter>()
        {
            new ReferenceFilter("<all>", string.Empty, string.Empty),
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
            if (item is ReferenceModel target)
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

        public ObservableCollection<ReferenceModel> AvailableReferences { get; }
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
            AvailableReferences = new ObservableCollection<ReferenceModel>();
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
            List<ReferenceModel> refItems = BeatSaberTools.GetAvailableReferences(BeatSaberDir);
            Project buildProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectFilePath).First();
            ProjectItem[] references = buildProject.Items.Where(obj => obj.ItemType == "Reference").ToArray();
            Dictionary<string, ReferenceModel> projRefs = new Dictionary<string, ReferenceModel>();
            foreach (ProjectItem item in references)
            {
                ReferenceModel refModel = new ReferenceModel(item.UnevaluatedInclude);
                if (item.HasMetadata("HintPath"))
                    refModel.HintPath = item.Metadata.Where(m => m.Name == "HintPath").First().UnevaluatedValue;
                if (item.HasMetadata("IncludeStripped"))
                {
                    string strippedVal = item.Metadata.First(m => m.Name == "IncludeStripped").EvaluatedValue;
                    refModel.IncludeStripped = bool.TryParse(strippedVal, out bool result) && result;
                }
                refModel.IsInProject = true;
                refModel.ResetModified();
                if (projRefs.TryGetValue(refModel.Name, out ReferenceModel existing))
                {
                    existing.IncludeStripped = existing.IncludeStripped || refModel.IncludeStripped;
                    if (string.IsNullOrEmpty(existing.HintPath))
                        existing.HintPath = refModel.HintPath;
                    existing.WarningStr = $"Project contains more than one reference to {refModel.Name}";
                }
                else
                    projRefs.Add(refModel.Name, refModel);
            }
            foreach (ReferenceModel item in refItems.ToArray())
            {
                if (item == null)
                    continue;
                if (projRefs.TryGetValue(item.Name, out ReferenceModel existing))
                {
                    item.CloneFrom(existing);
                }

                item.ResetModified();
                item.PropertyChanged += Item_PropertyChanged;
                AvailableReferences.Add(item);
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ReferenceModel referenceModel)
            {
                CheckChangedReferences();
            }
        }

        public void CheckChangedReferences()
        {
            ReferencesChanged = AvailableReferences.Any(r => r.IsModified);
        }

        public void UpdateReferences()
        {
            ReferenceModel[] changedRefs = AvailableReferences.Where(r => r.IsModified).ToArray();
            ReferenceModel[] removedRefs = changedRefs.Where(r => !r.IsInProject).ToArray();
            ReferenceModel[] addOrUpdatedRefs = changedRefs.Where(r => r.IsInProject).ToArray();
            foreach (ReferenceModel item in removedRefs)
            {
                Reference reference = _project.References.Find(item.Name);
                reference.Remove();
                item.IsInProject = false;
                item.ResetModified();
            }
            foreach (ReferenceModel item in addOrUpdatedRefs)
            {
                //var refPath = item.HintPath.Replace(BeatSaberDir, "$(BeatSaberDir)");
                if (_project.References.Find(item.Name) == null)
                {
                    Reference reference = _project.References.Add(item.HintPath);
                    reference.CopyLocal = true;
                }
                item.IsInProject = true;
            }
            Project buildProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectFilePath).First();
            foreach (ReferenceModel item in addOrUpdatedRefs)
            {
                ProjectItem newRef = buildProject.Items.Where(obj => obj.ItemType == "Reference" && GetSimpleReferenceName(obj.EvaluatedInclude) == item.Name).First();
                if (newRef != null)
                {
                    newRef.SetMetadataValue("HintPath", $"$(BeatSaberDir)\\{item.RelativeDirectory}\\{item.Name}.dll");
                    newRef.SetMetadataValue("Private", "True");
                    newRef.SetMetadataValue("SpecificVersion", "False");
                    if (item.IncludeStripped)
                        newRef.SetMetadataValue("IncludeStripped", item.IncludeStripped.ToString());
                    else
                        newRef.RemoveMetadata("IncludeStripped");
                    item.ResetModified();
                }
            }
            buildProject.MarkDirty();
            CheckChangedReferences();
        }
        private string GetSimpleReferenceName(string fullInclude)
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
        public string Prefix { get; }
        public ReferenceFilter(string name, string relativeDir, string prefix)
        {
            Name = name;
            RelativeDir = relativeDir;
            Prefix = prefix;
        }

        public Func<ReferenceModel, bool> IsMatch { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}