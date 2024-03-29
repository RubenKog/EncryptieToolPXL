using System.Windows;
using System.Windows.Navigation;
using EncryptieTool.Services;

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
            Navigation.SetMainFrame(MainFrame);
            Navigation.Navigate("MainView");
        }

        private void NavMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //< Get the clicked item
            var item = sender as System.Windows.Controls.MenuItem;

            //> Navigate to page
            Navigation.Navigate(item.Tag.ToString());
        }

        private void About_Hyperlink(object sender, RequestNavigateEventArgs e)
        {
            //> Open the about window
            new AboutWindow().ShowDialog();
        }
    }
}