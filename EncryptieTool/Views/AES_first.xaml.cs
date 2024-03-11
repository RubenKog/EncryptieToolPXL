using KeysLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using KeysLibrary;

namespace EncryptieTool.Views
{
    /// <summary>
    /// Interaction logic for AES_first.xaml
    /// </summary>
    public partial class AES_first : Window
    {
        string B64Text = string.Empty;
        public AES_first()
        {
            InitializeComponent();
        }
        private void BtnUseAES_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Use";

        }

        private void BtnGenAES_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Gen";

        }

        private void BtnUglyTest_Click(object sender, RoutedEventArgs e)
        {
            List<string> toDisplayList = CryptingAES.EncryptThenDecrypt();
            string ToEncrpyt = toDisplayList[0];
            string Encryted = toDisplayList[1];
            string Decrypted = toDisplayList[2];
            string KeyofAES = toDisplayList[3];
            string IVofAES = toDisplayList[4];

            System.Windows.Forms.MessageBox.Show($"string to encryt: {ToEncrpyt}");
            System.Windows.Forms.MessageBox.Show($"After encryption: {Encryted}");
            System.Windows.Forms.MessageBox.Show($"after decryption: {Decrypted}");
            System.Windows.Forms.MessageBox.Show($"AES key: {KeyofAES}");
            System.Windows.Forms.MessageBox.Show($"AES Initialization Vector: {IVofAES}");



        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                CryptingAES.ImageEncoded = Convert.ToBase64String(imageBytes);

                //Console.WriteLine(CryptingAES.ImageEncoded);
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
        }
    }
}
