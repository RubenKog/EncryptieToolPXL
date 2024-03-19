using System;
using System.Windows;
using System.Windows.Forms;
using EncryptieTool.Views;
using KeysLibrary;

namespace EncryptieTool
{
    public partial class MainWindow : Window
    {
        RSA_First rsaWindow;
        AES_first aesWindow;

        public MainWindow()
        {
            InitializeComponent();
            if (ChosenKey.FilePath != null)
            {
                BtnFolder.Content = "Change";
                LbFolder.Visibility = Visibility.Visible;
                LbFolder.Content = ChosenKey.FilePath;
            }

            Closing += MainWindow_Closing;
        }

        private void BtnAES_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.KeyType = "AES";
            aesWindow = new AES_first();
            aesWindow.Closed += ChildWindow_Closed;
            this.Content = aesWindow.Content;
        }

        private void BtnRSA_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.KeyType = "RSA";
            rsaWindow = new RSA_First();
            rsaWindow.Closed += ChildWindow_Closed; 
            this.Content = rsaWindow.Content;
        }

        private void BtnFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();

            // Display the dialog and await the user's choice
            DialogResult result = folderDialog.ShowDialog();

            // If the user selects a folder, save its path
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPaths.SelectedKeyFolder = folderDialog.SelectedPath;
                LbIsFolderSelected.Content = "Folder Selected!";
                LbFolder.Content= folderDialog.SelectedPath;
                LbFolder.Visibility = Visibility.Visible;
                BtnAES.IsEnabled = true;
                BtnRSA.IsEnabled = true;

                // Now you can save the selectedFolder path for later use
                // For example, you can store it in a variable or persist it to a file
                // Handle the sacred path according to the will of the Omnissiah
            }




           /* using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    ChosenKey.FilePath = folderDialog.SelectedPath;
                }
            }
            if (ChosenKey.FilePath != null)
            {
                BtnFolder.Content = "Change";
                LbFolder.Visibility = Visibility.Visible;
                LbFolder.Content = ChosenKey.FilePath;
            }*/
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {
            //Nog niet nodig, maar wis dit nog niet
        }
    }
}
