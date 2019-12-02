using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using BeatSaberModdingTools.Models;
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
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowser.ShowNewFolderButton = false;
                folderBrowser.Description = "Select your Beat Saber game folder.";
                var result = folderBrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(folderBrowser.SelectedPath))
                {
                    WindowViewModel.NewLocationInput = folderBrowser.SelectedPath;
                    LocationInput.Focus();
                }
            }
        }
    }

}
