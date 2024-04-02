using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using EncryptieTool.Services;
using EncryptieTool.Windows;
using KeysLibrary;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EncryptieTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            //> Initialize the directories
            Directories.Initialize();

            //> Set the main frame & Navigate to the main view
            Navigation.SetMainFrame(MainFrame);
            Navigation.Navigate("MainView");
        }

        #region Menu Items

        private void NavMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //< Get the clicked item
            var item = sender as System.Windows.Controls.MenuItem;

            //> Navigate to page
            Navigation.Navigate(item.Tag.ToString());
        }

        private void SelectPathClick(object sender, RoutedEventArgs e)
        {
            //: Prep
            string path = string.Empty;
            var item = (System.Windows.Controls.MenuItem)sender;
            string tag = item.Tag.ToString();

            //- Open file dialog
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select a folder";
                dialog.ShowDialog();
                path = dialog.SelectedPath;
            }

            //! Return if no path was selected
            if (string.IsNullOrEmpty(path)) return;

            //> Set the path based on tag
            switch (tag)
            {
                case "Key":
                    Directories.KeyFolderPath = path;
                    break;
                case "Encrypt":
                    Directories.EncryptFolderPath = path;
                    break;
                case "Decrypt":
                    Directories.DecryptedFolderPath = path;
                    break;
                default:
                    MessageBox.Show(
                        "Something went wrong while setting the path.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            //> Save the paths
            Directories.SavePaths();

            //> Refresh the pages
            Navigation.ReloadPages();
        }

        private void PathDetailsClick(object sender, RoutedEventArgs e)
        {
            new PathDetailsWindow().ShowDialog();
        }

        private void DisplayUIClick(object sender, RoutedEventArgs e)
        {
            //Get the clicked item
            var item = (System.Windows.Controls.MenuItem)sender;

            //Toggle MainFrame Nav UI visibility
            MainFrame.NavigationUIVisibility =
                (item.IsChecked) ? NavigationUIVisibility.Visible : NavigationUIVisibility.Hidden;
        }

        private void BackItemClick(object sender, RoutedEventArgs e)
        {
            Navigation.GoBack();
        }

        private void RefreshItemClick(object sender, RoutedEventArgs e)
        {
            Navigation.ReloadPages();
        }

        #endregion


        private void About_Hyperlink(object sender, RequestNavigateEventArgs e)
        {
            //> Open the about window
            new AboutWindow().ShowDialog();
        }
    }
}