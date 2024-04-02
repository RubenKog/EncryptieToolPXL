using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using EncryptieTool.Services;
using KeysLibrary;

namespace EncryptieTool.Windows
{
    public partial class PathDetailsWindow : Window
    {
        public PathDetailsWindow()
        {
            InitializeComponent();
            InitGUI();
        }

        private void InitGUI()
        {
            TxtKeyPath.Text = Directories.KeyFolderPath;
            TxtEncryptPath.Text = Directories.EncryptFolderPath;
            TxtDecryptPath.Text = Directories.DecryptedFolderPath;
        }

        private void ButtonReturn(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenPathInExplorer(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;

            if (border.Tag.ToString() == "EncryptFolder")
                System.Diagnostics.Process.Start("explorer.exe", Directories.EncryptFolderPath);
            else if (border.Tag.ToString() == "DecryptFolder")
                System.Diagnostics.Process.Start("explorer.exe", Directories.DecryptedFolderPath);
            else if (border.Tag.ToString() == "KeyFolder")
                System.Diagnostics.Process.Start("explorer.exe", Directories.KeyFolderPath);
        }

        private void ButtonRootFolder(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Directories.RootFolderPath);
        }

        private void ButtonReset(object sender, RoutedEventArgs e)
        {
            // Confirmation dialog
            var result = System.Windows.MessageBox.Show(
                "Are you sure you want to reset the paths to the default values?", 
                "Reset Paths", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;
            
            // Reset paths
            Directories.ResetPaths();
            
            // Refresh
            Navigation.ReloadPages();
            InitGUI();
        }
    }
}