using System;
using UnityModdingTools.Common.MVVM;
using UnityModdingTools.Projects;
#nullable enable

namespace UnityModdingTools.ReferenceManager.ViewModels
{
    public class ReferenceViewModel : ViewModelBase
    {
        private readonly ReferenceModel _ref;

        public string Name => _ref.Name;
        private string? _hintPath;

        public string? HintPath
        {
            get { return _hintPath; }
            set
            {
                if (_hintPath == value)
                    return;
                _hintPath = value;
                NotifyPropertyChanged();
                SetAndNotifyIsDirty();
            }
        }

        private CopyLocal _private;

        public CopyLocal Private
        {
            get { return _private; }
            set
            {
                if (_private == value)
                    return;
                _private = value;
                NotifyPropertyChanged();
                SetAndNotifyIsDirty();
            }
        }

        public string? Condition => _ref.Condition;

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty == value)
                    return;
                _isDirty = value;
                NotifyPropertyChanged();
                ResetCommand.RaiseCanExecuteChanged();
            }
        }

        public bool StartedInProject
        {
            get { return _ref.StartedInProject; }
            set
            {
                if (_ref.StartedInProject == value)
                    return;
                _ref.StartedInProject = value;
                NotifyPropertyChanged();
            }
        }

        public ReferenceViewModel(ReferenceModel referenceModel)
        {
            _ref = referenceModel ?? throw new ArgumentNullException(nameof(referenceModel));

            ResetCommand = new RelayCommand(() =>
            {
                HintPath = _ref.HintPath;
                Private = _ref.Private;
            }, () => IsDirty);
        }

        protected void SetAndNotifyIsDirty()
        {
            bool isDirty = HintPath != _ref.HintPath || Private != _ref.Private;
            IsDirty = isDirty;
        }

        public RelayCommand ResetCommand { get; }
    }
}
