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
using System.Security.Cryptography;
using Path = System.IO.Path;
using System.Drawing;

namespace EncryptieTool.Views
{
    /// <summary>
    /// Interaction logic for AES_first.xaml
    /// </summary>
    public partial class AES_first : Window
    {
        string B64Text = string.Empty;
        CryptingAES cryptingAES;
        bool ImageSelected;
        List<AesInfo> AesList;
        List<EncryptedImgInfo> EncryptedImgInfo;

        public AES_first()
        {
            ImageSelected = false;
            cryptingAES = new CryptingAES();
            AesList = new List<AesInfo>();
            EncryptedImgInfo = new List<EncryptedImgInfo>();
            InitializeComponent();
            ReadAesKeyFromFile();
        }
        private void BtnEncryptAES_Click(object sender, RoutedEventArgs e)
        {

            if (ImageSelected)
            {
                ChosenKey.Method = "Encrypt";
                if (cryptingAES.ImageEncoded != null)
                {
                    int IndexPicked = TxtEncrypt.SelectedIndex;

                    /*                    TxtDecrypt.Items.Add(cryptingAES.Encrypt(cryptingAES.ImageEncoded, AesList[IndexPicked] ));
                    */
                    try { 
                    
                    
                        SaveCryptedImg(cryptingAES.Encrypt(cryptingAES.ImageEncoded, AesList[IndexPicked]));
                        System.Windows.MessageBox.Show("Image succesfully encrypted.");

                    }

                    catch {
                        System.Windows.MessageBox.Show("Oops, something went wrong.");                   
                    
                    
                    }
                    
                }
                else
                {
                    System.Windows.MessageBox.Show("Please select an image to encrypt");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an image to encrypt");
            }
           
        }

        private void SaveCryptedImg(string EncryptedImg)
        {
            string imgName = TxtImgName.Text;
            string filePath = Path.Combine(SelectedPaths.SelectedSaveEncryptFolder, "EncryptedImg.txt");
            using (StreamWriter writer = File.AppendText(filePath))
            {
                // Write the AES information as a group of three
                writer.WriteLine($"Img Name: {imgName}");
                writer.WriteLine($"AES-Encrypted Image: {EncryptedImg}");
                writer.WriteLine(); // Add an empty line to separate groups
            }
            ReadCryptedImg();



        }
        private void ReadCryptedImg()
        {
            string filePath = Path.Combine(SelectedPaths.SelectedSaveEncryptFolder, "EncryptedImg.txt");
            if (File.Exists(filePath))
            {
                EncryptedImgInfo.Clear();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    string imgName = null, imgBase64 = null;

                    // Read each line and extract the AES information
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("Img Name:"))
                        {
                            imgName = line.Split(':')[1].Trim();
                        }
                        else if (line.StartsWith("AES-Encrypted Image"))
                        {
                            imgBase64 = line.Split(':')[1].Trim();
                        }
                        
                        else if (string.IsNullOrWhiteSpace(line) && imgBase64 != null && imgName != null)
                        {
                            EncryptedImgInfo info = new EncryptedImgInfo()
                            {
                                ImgName = imgName,
                                EncryptedImg = imgBase64,
                            };

                            EncryptedImgInfo.Add(info);
                            imgName = imgBase64 = null; // Reset for the next group
                        }
                    }
                }

                List<string> nameList = new List<string>();
                foreach (EncryptedImgInfo info in EncryptedImgInfo)
                {
                    nameList.Add(info.ImgName);
                }
                TxtDecrypt.ItemsSource = nameList;
            }
        }


        private void SaveDecryptedImg(string decryptedImg)
        {
            // Decode the Base64 string to bytes
            byte[] imageBytes = Convert.FromBase64String(decryptedImg);

            // Create a MemoryStream to hold the image bytes
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Create an Image from the MemoryStream
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                

                // Combine the chosen path and image name to form the full file path
                string filePath = Path.Combine(SelectedPaths.SelectedSaveEncryptFolder, $"{TxtImgName.Text}.png");

                // Save the image to the specified path
                image.Save(filePath);
            }


        }



        private void BtnDecryptAES_Click(object sender, RoutedEventArgs e)
        {
            ChosenKey.Method = "Decrypt";
            if (cryptingAES.ImageEncoded != null)
            {
                try
                {
                    int indexPickedAes = TxtEncrypt.SelectedIndex;
                    int indexPickedImg = TxtDecrypt.SelectedIndex;
                    /*TxtDecrypt.Items.Add(cryptingAES.Decrypt(cryptingAES.ImageEncoded));*/
                    SaveDecryptedImg(cryptingAES.Decrypt(EncryptedImgInfo[indexPickedImg].EncryptedImg, AesList[indexPickedAes]));
                    System.Windows.MessageBox.Show("Decryption succesful, image saved!");

                }
                catch 
                {
                    System.Windows.MessageBox.Show("Oops, something went wrong. You might have selected the wrong key to decrypt this file.");
                }



                
            }
            else
            {
                System.Windows.MessageBox.Show("Please select an encrypted image to decrypt.");
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
            
            openFileDialog.Filter = "Image Files (*.png;)|*.png;";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);

                BitmapImage NewImg = new BitmapImage();
                NewImg.BeginInit();
                NewImg.CacheOption = BitmapCacheOption.OnLoad;
                NewImg.StreamSource = new MemoryStream(imageBytes);
                NewImg.EndInit();

                PickedImg.Source = NewImg;
                cryptingAES.ImageEncoded = Convert.ToBase64String(imageBytes);

                #region FileName
                string filePath = openFileDialog.FileName;
                int lastSlashIndex = Math.Max(filePath.LastIndexOf('\\'), filePath.LastIndexOf('/'));
                string result = (lastSlashIndex >= 0) ? filePath.Substring(lastSlashIndex + 1) : filePath;
                lbImageName.Content = result;
                #endregion

                Console.WriteLine(cryptingAES.ImageEncoded);                
                ImageSelected = true;
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
        }

        private void BtnImgFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();

            // Display the dialog and await the user's choice
            DialogResult result = folderDialog.ShowDialog();

            // If the user selects a folder, save its path
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPaths.SelectedSaveEncryptFolder = folderDialog.SelectedPath;
                LbIsImgFolderSelected.Content = "Folder Selected!";
                LbImgFolder.Content = folderDialog.SelectedPath;
                LbImgFolder.Visibility = Visibility.Visible;
                BtnSelectImage.IsEnabled = true;
                ReadCryptedImg();

                // Now you can save the selectedFolder path for later use
                // For example, you can store it in a variable or persist it to a file
                // Handle the sacred path according to the will of the Omnissiah
            }




        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {            
            Aes aes = cryptingAES.CreateAES();
            string aesKey = cryptingAES.ByteArToReadableString(aes.Key);
            string aesIV = cryptingAES.ByteArToReadableString(aes.IV);
            string aesName = LblAesName.Text;

            string filePath = Path.Combine(SelectedPaths.SelectedKeyFolder, "AESInfo.txt");

            // Write the AES information to the file
            using (StreamWriter writer = File.AppendText(filePath))
            {
                // Write the AES information as a group of three
                writer.WriteLine($"AES Name: {aesName}");
                writer.WriteLine($"AES Key: {aesKey}");
                writer.WriteLine($"AES IV: {aesIV}");
                writer.WriteLine(); // Add an empty line to separate groups
            }
            ReadAesKeyFromFile();


        }

        private void ReadAesKeyFromFile()
        {
            string filePath = Path.Combine(SelectedPaths.SelectedKeyFolder, "AESInfo.txt");
            if (File.Exists(filePath))
            {
                AesList.Clear();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    string aesName = null, aesKey = null, aesIV = null;

                    // Read each line and extract the AES information
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("AES Name:"))
                        {
                            aesName = line.Split(':')[1].Trim();
                        }
                        else if (line.StartsWith("AES Key:"))
                        {
                            aesKey = line.Split(':')[1].Trim();
                        }
                        else if (line.StartsWith("AES IV:"))
                        {
                            aesIV = line.Split(':')[1].Trim();
                        }
                        else if (string.IsNullOrWhiteSpace(line) && aesName != null && aesKey != null && aesIV != null)
                        {
                            AesInfo info = new AesInfo()
                            {
                                AesName = aesName,
                                AesKey = aesKey,
                                AesIV = aesIV
                            };

                            AesList.Add(info);
                            aesName = aesKey = aesIV = null; // Reset for the next group
                        }
                    }
                }

                List<string> nameList = new List<string>();
                foreach( AesInfo aesInfo  in AesList)
                {
                    nameList.Add(aesInfo.AesName);
                }
                TxtEncrypt.ItemsSource = nameList;
            }

        }

        private void TxtEncrypt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TxtEncrypt.SelectedIndex == -1)
            {
                BtnEncryptAESx.IsEnabled = false;
            }
            else if(ImageSelected == true)
            {
                BtnEncryptAESx.IsEnabled = true;
            }
            else
            {
                TxtEncrypt.SelectedIndex = -1;
            }
        }

        private void TxtDecrypt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtDecrypt.SelectedIndex == -1)
            {
                BtnDecryptAESx.IsEnabled = false;
            }
            else
            {
                BtnDecryptAESx.IsEnabled = true;
            }
        }
    }
}
