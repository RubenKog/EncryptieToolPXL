using KeysLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EncryptieTool.Views
{
    /// <summary>
    /// Interaction logic for RSA_First.xaml
    /// </summary>
    public partial class RSA_First : Window
    {
        public RSA_First()
        {
            InitializeComponent();
        }

        private void BtnUseRSA_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Use";

        }

        private void BtnGenRSA_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Gen";

        }
    }
}
