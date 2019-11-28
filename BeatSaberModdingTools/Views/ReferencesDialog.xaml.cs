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
namespace BeatSaberModdingTools.Views
{
    /// <summary>
    /// Interaction logic for ReferencesDialog.xaml
    /// </summary>
    public partial class ReferencesDialog : Window
    {
        public ReferenceWindowViewModel ViewModel;
        public ReferencesDialog(string projectFilePath)
        {
            ViewModel = new ReferenceWindowViewModel(projectFilePath, BSMTSettingsManager.Instance.CurrentSettings.ChosenInstallPath);
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
