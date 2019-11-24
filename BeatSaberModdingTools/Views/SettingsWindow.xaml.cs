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
using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.ViewModels;
using Microsoft.VisualStudio.PlatformUI;

namespace BeatSaberModdingTools.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        WindowViewModel WindowViewModel;
        public SettingsWindow()
        {
            WindowViewModel = new WindowViewModel();
            DataContext = WindowViewModel;
            InitializeComponent();
        }

        void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            BSMTSettingsManager.Store(WindowViewModel.SettingsViewModel.CurrentSettings);
            BSMTSettingsManager.Reload();
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public SettingsModel ReturnSettings
        {
            get { return WindowViewModel.SettingsViewModel.CurrentSettings; }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is DataGridRow dataGridRow && dataGridRow.DataContext is BeatSaberInstall install)
            {
                WindowViewModel.ChosenInstall = install;
                e.Handled = true;
            }
        }
    }

}
