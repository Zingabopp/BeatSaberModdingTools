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
        public ObservableCollection<string> PathOptions { get; } = new ObservableCollection<string>(PathProperties.Append("None").ToArray());
        public string ProjectFilePath { get; private set; }
        public string BeatSaberDir { get; private set; }

        private readonly HashSet<string> ProjectProperties = new HashSet<string>();

        private string _pathOption;

        public string PathOption
        {
            get { return _pathOption; }
            set
            {
                WarningText = GetDisplayPathWarningText(value);
                if (_pathOption == value) return;
                _pathOption = value;
                NotifyPropertyChanged();
            }
        }

        private string GetDisplayPathWarningText(string value)
        {
            if (ProjectProperties.Count > 0)
            {
                if (value == "None")
                    return $"Path option 'None' will use absolute paths for references, it is recommended to use '{ProjectProperties.First()}'.";
                if (!ProjectProperties.Contains(value))
                    return $"Path option '{value}' does not appear to be used by the project.";
                if (ProjectProperties.First() != value)
                    return $"Path option '{ProjectProperties.First()}' is recommended for this project.";
            }
            else if (value != "None")
                return $"Path option '{value}' does not appear to be used by the project.";
            if (IsSDK)
                return "SDK projects are not fully supported. You will have to manually change the HintPath for added references.";
            return string.Empty;
        }

        private string _warningText;

        public string WarningText
        {
            get { return _warningText; }
            set
            {
                if (_warningText == value) return;
                _warningText = value;
                NotifyPropertyChanged();
            }
        }


        public ReferenceWindowViewModel()
        {
            AvailableReferences = DesignExample;
            _projectName = "DesignerTest";
            BeatSaberDir = @"C:\TestBeatSaberDir";
            PathOption = "BeatSaberReferences";
            SelectedFilter = Filters.First();
            //Refresh.Execute(null);
        }

        public static bool IsSDKProject(Project project)
        {
            if (project == null)
                return false;
            ProjectProperty prop = project.GetProperty("UsingMicrosoftNetSdk");
            string val = prop?.EvaluatedValue;
            return val != null && val.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsSDK { get; set; }
        private Project _evaluationProject;

        public Project EvaluationProject
        {
            get
            {
                if (_evaluationProject == null && !string.IsNullOrWhiteSpace(ProjectFilePath))
                    _evaluationProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectFilePath).FirstOrDefault();
                return _evaluationProject;
            }
            set { _evaluationProject = value; }
        }

        public ReferenceWindowViewModel(string projectFilePath, string projectName, VSProject project, Project evalProject, string beatSaberDir)
        {
            AvailableReferences = new ObservableCollection<ReferenceItemViewModel>();
            ProjectFilePath = projectFilePath;
            EvaluationProject = evalProject;
            UpdateAvailableReferenceRoots(evalProject);
            IsSDK = IsSDKProject(evalProject);
            if (ProjectProperties.Count > 0)
                PathOption = ProjectProperties.First();
            else
                PathOption = "None";
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
            var buildProject = EvaluationProject;
            if(buildProject == null)
                WarningText = $"Could not load project information from '{ProjectFilePath}'";
            ProjectItem[] references = buildProject?.Items.Where(obj => obj.ItemType == "Reference").ToArray() ?? Array.Empty<ProjectItem>();
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
            Dictionary<string, Reference> references = new Dictionary<string, Reference>();
            foreach (Reference reference in _project.References)
            {
                if (!references.ContainsKey(reference.Name))
                    references.Add(reference.Name, reference);
            }
            foreach (var item in removedRefs)
            {
                if (references.TryGetValue(item.Name, out Reference reference))
                {
                    reference.Remove();
                    item.StartedInProject = false;
                    item.IsInProject = false;
                }
            }
            var addedRefs = changedRefs.Where(r => r.IsInProject).ToArray();
            foreach (var item in addedRefs)
            {
                //var refPath = item.HintPath.Replace(BeatSaberDir, "$(BeatSaberDir)");
                if (!references.TryGetValue(item.Name, out Reference reference))
                {
                    reference = _project.References.Add(item.HintPath);
                    reference.CopyLocal = false;
                    item.StartedInProject = true;
                }
                else
                {
                    item.StartedInProject = true;
                    item.IsInProject = true;
                }
            }

            var buildProject = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectFilePath).FirstOrDefault();
            if (buildProject != null)
            {
                string rootPath = GetReferenceRootPath(buildProject);
                foreach (var item in addedRefs)
                {
                    var newRef = buildProject.Items.Where(obj => obj.ItemType == "Reference" && GetSimpleReferenceName(obj.EvaluatedInclude) == item.Name).FirstOrDefault();
                    string hintPath = item.HintPath;
                    if (rootPath != null)
                        hintPath = $"{rootPath}\\{item.RelativeDirectory}\\{item.Name}.dll";
                    KeyValuePair<string, string>[] meta = new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>("HintPath", hintPath),
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
            }
            //buildProject.ReevaluateIfNecessary();
            //buildProject.Save();
            CheckChangedReferences();
        }
        private static readonly string[] PathProperties = new string[] { "BeatSaberReferences", "BeatSaberDir" };
        private void UpdateAvailableReferenceRoots(Project project)
        {
            if (project == null)
                return;
            ProjectProperties.Clear();
            ProjectProperty prop = null;
            for (int i = 0; i < PathProperties.Length; i++)
            {
                prop = project.GetProperty(PathProperties[i]);
                if (prop != null && !string.IsNullOrWhiteSpace(prop.UnevaluatedValue))
                {
                    ProjectProperties.Add(prop.Name);
                }
            }
            PathOption = PathOption;
        }

        private string GetReferenceRootPath(Project project)
        {
            if (PathOption == "None" || PathOption == null)
                return null;
            return $"$({PathOption})";
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