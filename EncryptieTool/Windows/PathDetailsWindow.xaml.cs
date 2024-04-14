using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using EncryptieTool.Services;
using KeysLibrary;
using KeysLibrary.Services;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;

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
            TxtAesPlain.Text = Directories.PlainAesPath;
            TxtAesCipher.Text = Directories.CipherAesPath;
            TxtAesCipherDecrypted.Text = Directories.DecryptedCipherAesPath;
            TxtRsaPublic.Text = Directories.RsaPublicPath;
            TxtRsaPrivate.Text = Directories.RsaPrivatePath;
            TxtEncryptedImage.Text = Directories.EncryptedImgPath;
            TxtDecryptedImage.Text = Directories.DecryptedImgPath;
        }

        private void ButtonReturn(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenPathInExplorer(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;

            switch (border.Tag.ToString())
            {
                case "AesPlain":
                    System.Diagnostics.Process.Start("explorer.exe", Directories.PlainAesPath);
                    break;
                case "AesCipher":
                    System.Diagnostics.Process.Start("explorer.exe", Directories.CipherAesPath);
                    break;
                case "AesCipherDecrypted":
                    System.Diagnostics.Process.Start("explorer.exe", Directories.DecryptedCipherAesPath);
                    break;
                case "RsaPublic":
                    System.Diagnostics.Process.Start("explorer.exe", Directories.RsaPublicPath);
                    break;
                case "RsaPrivate":
                    System.Diagnostics.Process.Start("explorer.exe", Directories.RsaPrivatePath);
                    break;
                case "ImgEncrypted":
                    System.Diagnostics.Process.Start("explorer.exe", Directories.EncryptedImgPath);
                    break;
                case "ImgDecrypted":
                    System.Diagnostics.Process.Start("explorer.exe", Directories.DecryptedImgPath);
                    break;
                default:
                    System.Diagnostics.Process.Start("explorer.exe", Directories.RootFolderPath);
                    break;
            }
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

        private void ButtonNewClick(object sender, RoutedEventArgs e)
        {
            // Get the clicked item
            var button = (Button)sender;

            //Open file dialog
            string path;
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select a folder";
                dialog.ShowDialog();
                path = dialog.SelectedPath;
            }

            //! Return if no path was selected
            if (string.IsNullOrEmpty(path)) return;

            // Set the path based on tag
            switch (button.Tag.ToString())
            {
                case "AesPlain":
                    Directories.PlainAesPath = path;
                    break;
                case "AesCipher":
                    Directories.CipherAesPath = path;
                    break;
                case "AesCipherDecrypted":
                    Directories.DecryptedCipherAesPath = path;
                    break;
                case "RsaPublic":
                    Directories.RsaPublicPath = path;
                    break;
                case "RsaPrivate":
                    Directories.RsaPrivatePath = path;
                    break;
                case "ImgEncrypted":
                    Directories.EncryptedImgPath = path;
                    break;
                case "ImgDecrypted":
                    Directories.DecryptedImgPath = path;
                    break;
                default:
                    MessageBox.Show(@"Button tag name not recognized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            
            //> Save the paths
            Directories.SavePaths();

            //> Refresh the pages
            Navigation.ReloadPages();
            
            //> Refresh the GUI
            InitGUI();
        }
    }
}