using KeysLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EncryptieTool.Views
{
    public partial class RSA_First : Window
    {
        public RSA_First()
        {
            InitializeComponent();
            InitializeGui();
        }

        #region Properties

        private const string DefaultKeyPathText = "\ud83d\udd11 Key:";
        private const string DefaultDestinationPathText = "\ud83d\udcc1 Destination:";
        private const string WarningImgPath = "pack://application:,,,/Images/Warning.png";
        private const string CheckImgPath = "pack://application:,,,/Images/Check.png";

        #endregion

        #region Ruben's Code

        private void BtnUseRSA_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Use";
        }

        private void BtnGenRSA_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Gen";
        }

        #endregion

        #region Misc

        private void InitializeGui()
        {
            //Display Paths
            TxtKeyPath.Text = string.IsNullOrEmpty(SelectedPaths.SelectedKeyFolder)
                ? $"{DefaultKeyPathText} Not selected."
                : $"{DefaultKeyPathText} {SelectedPaths.SelectedKeyFolder}";
            TxtDestinationPath.Text = $"{DefaultDestinationPathText} Not selected.";
        }

        /// <summary>
        /// Returns null if no path is selected.
        /// </summary>
        private static string PickFolderPath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                return result == System.Windows.Forms.DialogResult.OK ? dialog.SelectedPath : null;
            }
        }

        private void UpdateImgStatus(string path)
        {
            if (path is null)
                ImgStatus.Source = new BitmapImage(new Uri(WarningImgPath));
            else
                ImgStatus.Source = new BitmapImage(new Uri(CheckImgPath));
        }

        #endregion

        #region MenuItems

        private void FolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Select folder
            string path = PickFolderPath();

            //Return if null
            if (path is null)
                return;

            //Save path --according to corresponding sender
            var item = (System.Windows.Controls.MenuItem)sender;
            if (item.Tag.ToString().Contains("key"))
            {
                SelectedPaths.SelectedKeyFolder = path;
                TxtKeyPath.Text = $"{DefaultKeyPathText} {path}";
            }
            else if (item.Tag.ToString().Contains("destination"))
            {
                SelectedPaths.SelectedSaveEncryptFolder = path;
                TxtDestinationPath.Text = $"{DefaultDestinationPathText} {path}";
            }
        }

        private void ReturnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(@"Jah als ik eens wist hoe??");
        }

        #endregion
    }
}