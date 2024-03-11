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
        CryptingAES cryptingAES;
        public AES_first()
        {
            cryptingAES = new CryptingAES();
            InitializeComponent();
        }
        private void BtnEncryptAES_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Encrypt";
            if (cryptingAES.ImageEncoded != null)
            {
                TxtEncrypt.Items.Add(CryptingAES.Encrypt(cryptingAES.ImageEncoded));
            }
            else
            {
                System.Windows.MessageBox.Show("please select an image to encrypt");
            }
        }

        private void BtnDecryptAES_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Decrypt";
            if (cryptingAES.ImageEncoded != null)
            {
                TxtDecrypt.Items.Add(CryptingAES.Decrypt(cryptingAES.ImageEncoded));
            }
            else
            {
                System.Windows.MessageBox.Show("please select an image to encrypt");
            }
        }

        //private void BtnUglyTest_Click(object sender, RoutedEventArgs e)
        //{
        //    List<string> toDisplayList = CryptingAES.EncryptThenDecrypt("fd");
        //    string ToEncrpyt = toDisplayList[0];
        //    string Encryted = toDisplayList[1];
        //    string Decrypted = toDisplayList[2];
        //    string KeyofAES = toDisplayList[3];
        //    string IVofAES = toDisplayList[4];

        //    System.Windows.Forms.MessageBox.Show($"string to encryt: {ToEncrpyt}");
        //    System.Windows.Forms.MessageBox.Show($"After encryption: {Encryted}");
        //    System.Windows.Forms.MessageBox.Show($"after decryption: {Decrypted}");
        //    System.Windows.Forms.MessageBox.Show($"AES key: {KeyofAES}");
        //    System.Windows.Forms.MessageBox.Show($"AES Initialization Vector: {IVofAES}");
        //}

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                cryptingAES.ImageEncoded = Convert.ToBase64String(imageBytes);

                #region FileName
                string filePath = openFileDialog.FileName;
                int lastSlashIndex = Math.Max(filePath.LastIndexOf('\\'), filePath.LastIndexOf('/'));
                string result = (lastSlashIndex >= 0) ? filePath.Substring(lastSlashIndex + 1) : filePath;
                lbImageName.Content = result;
                #endregion

                Console.WriteLine(cryptingAES.ImageEncoded);
                BtnEncryptAESx.IsEnabled = true;
                BtnDecryptAESx.IsEnabled = true;
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
        }
    }
}
