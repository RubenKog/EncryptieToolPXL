using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using EncryptieTool.Services;
using KeysLibrary;

namespace EncryptieTool.Views
{
    public partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void AES_Click(object sender, RoutedEventArgs e)
        {
            // if (string.IsNullOrEmpty(Directories.SelectedKeyFolder))
            // {
            //     System.Windows.MessageBox.Show("Selecteer eerst een map om de AES keys in op te slaan.",
            //         "Geen map geselecteerd", MessageBoxButton.OK, MessageBoxImage.Warning);
            // }
            // else
            // {
            //     ChosenKey.KeyType = "AES";
            //     aesWindow = new AesView();
            //     this.Hide();
            //     aesWindow.Show();
            // }
            
            //ChosenKey.KeyType = "AES"; :|
            Navigation.Navigate("AesView");
        }

        private void RSA_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigate("RsaView");
        }

        private void BtnFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();

            // Display the dialog and await the user's choice
            DialogResult result = folderDialog.ShowDialog();

            // If the user selects a folder, save its path
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Directories.KeyFolderPath = folderDialog.SelectedPath;
                //LbIsFolderSelected.Content = "Folder Selected!";
                //LbFolder.Content = folderDialog.SelectedPath;
                //LbFolder.Visibility = Visibility.Visible;
                // Now you can save the selectedFolder path for later use
                // For example, you can store it in a variable or persist it to a file
                // Handle the sacred path according to the will of the Omnissiah
            }
        }
    }
}