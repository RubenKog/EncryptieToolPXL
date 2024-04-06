using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace EncryptieTool
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }
        
        List<string> sites = new List<string>
        {
            "https://theuselessweb.com/",
            "https://hackertyper.com/",
            "https://pointerpointer.com/",
            "https://tholman.com/cursor-effects/",
            "https://www.boredbutton.com/",
            "https://www.agegeek.com/",
            "https://screamintothevoid.com/",
            "http://dontevenreply.com/",
            "https://stellarium-web.org/",
        };

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //> Open a random link from the list
            System.Diagnostics.Process.Start(sites[new System.Random().Next(sites.Count)]);
            
            //Very important message
            MessageBox.Show("There's no reason for you to mail us, yeah?", "Mail",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}