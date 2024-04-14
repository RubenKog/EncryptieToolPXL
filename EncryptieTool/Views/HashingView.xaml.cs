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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeysLibrary.Services;

namespace EncryptieTool.Views
{
    /// <summary>
    /// Interaction logic for HashingView.xaml
    /// </summary>
    public partial class HashingView : Page
    {
        //FilePaths
        string File1;
        string File2;
        public string HashingAlgor { get; set; } = "SHA256";
        public HashingView()
        {
            File1 = null;
            File2 = null;
            InitializeComponent();
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hashing hashing = new Hashing();
                StringBuilder sb = new StringBuilder();
                byte[] HashedFile1;
                byte[] HashedFile2;
                string hashAlgorithm;
                string hashString1;
                string hashString2;

                // Determine hash algorithm
                if (HashingAlgor == "SHA256")
                {
                    HashedFile1 = hashing.FileToHashSHA(File1);
                    HashedFile2 = hashing.FileToHashSHA(File2);
                    hashAlgorithm = "SHA256";
                }
                else
                {
                    HashedFile1 = hashing.FileToHashBLAKE(File1);
                    HashedFile2 = hashing.FileToHashBLAKE(File2);
                    hashAlgorithm = "BLAKE";
                }

                // Check if files are selected
                if (HashedFile1 == null || HashedFile2 == null)
                {
                    MessageBox.Show("Please select two files.");
                    return;
                }

                // Convert hash bytes to strings
                hashString1 = Convert.ToBase64String(HashedFile1);
                hashString2 = Convert.ToBase64String(HashedFile2);

                sb.AppendLine($"Checking integrity through {hashAlgorithm} hash:");

                // Compare hashes and display result
                if (hashString1 == hashString2)
                {
                    sb.AppendLine("");
                    sb.AppendLine($"File 1 hash: {hashString1}");
                    sb.AppendLine("");
                    sb.AppendLine($"File 2 hash: {hashString2}");
                    sb.AppendLine("");
                    sb.AppendLine("Integrity check complete: Success!");
                }
                else
                {
                    sb.AppendLine("");
                    sb.AppendLine($"File 1 hash: {hashString1}");
                    sb.AppendLine("");
                    sb.AppendLine($"File 2 hash: {hashString2}");
                    sb.AppendLine("");
                    sb.AppendLine("Integrity check complete: Failure!");
                }

                MessageBox.Show(sb.ToString(), "Integrity Checker");
            }
            catch
            {
                MessageBox.Show("Please select two files.");
            }


        }

        private void BtnSelectFile1_Click(object sender, RoutedEventArgs e)
        {
            File1 = GetFilePath();
            TxtPlainImgName1.Text = File1;
        }
        private void BtnSelectFile2_Click(object sender, RoutedEventArgs e)
        {
            File2 = GetFilePath();
            TxtPlainImgName2.Text = File2;
        }

        private string GetFilePath()
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();

            
            openFileDialog.Title = "Select a file";
            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
              
                string filePath = openFileDialog.FileName;
                return filePath;
                
            }
            else
            {
                Console.WriteLine("No file selected.");
                return null;
            }
        }

        private void CBHashingAG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HashingAlgor = ((ComboBoxItem)CBHashingAG.SelectedItem).Content.ToString();
            if(HashingAlgor != "SHA256")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("BLAKE3 doesn't work in the same manner as SHA256.");
                sb.AppendLine("It's newer, providing a better resistance against various cryptographic attacks than SHA256.");
                sb.AppendLine("");
                sb.AppendLine("However, we have noticed that BLAKE3 can, unlike SHA256, be affected by the metadata of a file.");
                sb.AppendLine("This means that 2 files with the same content can result in 2 different hashes.");
                sb.AppendLine("So if an integrity test results in a fail while using BLAKE3, this might not be correct. False negatives are possible.");
                MessageBox.Show(sb.ToString(), "BLAKE3 warning");
            }
        }
    }
}