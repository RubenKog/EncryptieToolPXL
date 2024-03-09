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
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
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
            }
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
