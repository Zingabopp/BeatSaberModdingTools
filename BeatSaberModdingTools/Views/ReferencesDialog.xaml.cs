using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BeatSaberModdingTools.ViewModels;
using VSLangProj;

namespace BeatSaberModdingTools.Views
{
    /// <summary>
    /// Interaction logic for ReferencesDialog.xaml
    /// </summary>
    public partial class ReferencesDialog : Window
    {
        public ReferenceWindowViewModel ViewModel;
        public CollectionViewSource ReferencesView;
        public ReferencesDialog(VSProject project)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            ViewModel = new ReferenceWindowViewModel(project.Project.FullName, project.Project.Name, project, BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath);
            DataContext = ViewModel;
            InitializeComponent();
            ReferencesView = (CollectionViewSource)FindResource("ReferenceListSource");
            ViewModel.ReferenceView = ReferencesView.View;
            ViewModel.ReferenceView.Filter = ViewModel.Filter;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            ViewModel.ApplyChanges.Execute(null);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
