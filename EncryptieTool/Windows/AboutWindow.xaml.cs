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
            "https://www.ishetalvrijdag.nl/",
            "https://www.ishetalweekend.nl/",
            "https://www.ishetal5uur.nl/",
            "https://www.hoelaatwordthetdonker.nl/",
            "https://www.hoeheetishet.nl/",
            "https://www.watbenjedan.nl/",
            "https://www.watbenjedan.xyz/",
            "https://www.enwatbenjedan.nl/",
            "https://www.youtube.com/watch?v=hiRacdl02w4",
            "https://www.youtube.com/watch?v=hiRacdl02w4",
            "https://www.youtube.com/watch?v=hiRacdl02w4",
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