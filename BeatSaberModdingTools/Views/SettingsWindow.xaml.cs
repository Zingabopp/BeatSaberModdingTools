using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using BeatSaberModdingTools.ViewModels;

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
            BSMTSettingsManager.Instance.Store(WindowViewModel.SettingsViewModel.CurrentSettings);
            BSMTSettingsManager.Instance.Reload();
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

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderSelectDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Title = "Select your Beat Saber game folder."
            };
            if (dialog.Show())
            {
                WindowViewModel.NewLocationInput = dialog.FileName;
                LocationInput.Focus();
            }
        }
    }

}
