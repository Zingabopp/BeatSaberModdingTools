using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using BeatSaberModdingTools.Models;
using BeatSaberModdingTools.Utilities;
using BeatSaberModdingTools.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace BeatSaberModdingTools.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        WindowViewModel WindowViewModel;
        WindowInteropHelper InteropHelper;
        public SettingsWindow(AsyncPackage package)
        {
            WindowViewModel = new WindowViewModel(new NotificationHandler(package));
            DataContext = WindowViewModel;
            InitializeComponent();
            InteropHelper = new WindowInteropHelper(this);
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
            btnBrowse.IsEnabled = false;
            string dir = WindowViewModel.NewLocationInput;
            if (dir == null || dir.Length == 0 || !Directory.Exists(dir))
                dir = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            var dialog = new FolderSelectDialog
            {
                InitialDirectory = dir,
                Title = "Select your Beat Saber game folder."
            };
            if (dialog.Show(InteropHelper.Handle))
            {
                WindowViewModel.NewLocationInput = dialog.FileName;
                LocationInput.Focus();
            }
            btnBrowse.IsEnabled = true;
        }
    }

}
