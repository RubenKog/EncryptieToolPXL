using KeysLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using EncryptieTool.Windows;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;
using System.Diagnostics;
using System.Text;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace EncryptieTool.Views
{
    public partial class AesView : Page
    {
        #region Properties

        private CryptingAES _cryptingAes = new CryptingAES();
        private List<AesInfo> _aesList = new List<AesInfo>();
        private List<EncryptedImgInfo> _encryptedImgInfo = new List<EncryptedImgInfo>();
        private string _b64Text = string.Empty; // Never used?

        private string _keyName = string.Empty;
        private string _encryptedImgName = string.Empty; //Name for the image that is being encrypted
        private string _decryptedImgName = string.Empty; //Name for the image that is being decrypted
        
        bool OpenFolder { get; set; }
        string filePathA { get; set; }

        #endregion

        public AesView()
        {
            InitializeComponent();
        }

        #region Misc

        private void AesView_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Initialize the GUI when the page is requested.
            Init();
        }

        private void Init()
        {
            // Read keys and encrypted images
            ReadAesKeyFromFile();
            ReadEncryptedImg();
        }

        private void ClearKeyName()
        {
            _keyName = string.Empty;
            TxtKeyName.Text = "Click to Name Key";
            TxtKeyName.Foreground = System.Windows.Media.Brushes.LightSteelBlue;
        }
        
        private void CheckDefaultImgName_Checked(object sender, RoutedEventArgs e)
        {
            TxtDecryptedImgName.Visibility = Visibility.Visible;
        }

        private void CheckDefaultImgName_Unchecked(object sender, RoutedEventArgs e)
        {
            TxtDecryptedImgName.Visibility = Visibility.Collapsed;
        }

        private void CheckOpenFolder_Checked(object sender, RoutedEventArgs e)
        {
            OpenFolder = true;
        }

        private void CheckOpenFolder_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenFolder = false;
        }

        #endregion

        #region Encrypting/Decrypting

        private void BtnEncryptAES_Click(object sender, RoutedEventArgs e)
        {
            //Check if it's okay to encrypt
            if (!IsOkToEncrypt()) return;


            // Set the method to Encrypt
            int IndexPicked = ListKeys.SelectedIndex;
            try
            {
                SaveEncryptedImg(_cryptingAes.Encrypt(_cryptingAes.ImageEncoded, _aesList[IndexPicked]));
                System.Windows.MessageBox.Show("Encryption successful, image saved!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear the name
                _encryptedImgName = string.Empty;
                TxtEncryptedImgName.Text = "Click to Name Image";
                TxtEncryptedImgName.Foreground = System.Windows.Media.Brushes.LightSteelBlue;

                // Unselect Items in listbox'
                ListKeys.SelectedIndex = -1;
            }
            catch (CryptographicException ex)
            {
                System.Windows.MessageBox.Show("Oops, something went wrong.\n" +
                                               ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerateKey_Click(object sender, RoutedEventArgs e)
        {
            //! Check if a proper name has been given to key.
            if (string.IsNullOrEmpty(_keyName))
            {
                //? Ask if user wants to name the key now.
                var result = MessageBox.Show(
                    "No name for key was given. Would you like to name it now?",
                    "Nameless key?",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);

                //! If user doesn't want to name the key now, return.
                if (result == MessageBoxResult.No) return;

                //> Else, open the InputWindow and await the user's input
                AssignKeyName();

                //! If the user still didn't give a name, return. (bruh moment -.- we got prank'd innit) 
                if (string.IsNullOrEmpty(_keyName)) return;
            }

            //! Check if the keyName is already in use
            if (_aesList.Any(x => x.AesName.ToUpper() == _keyName.ToUpper()))
            {
                MessageBox.Show("This key name is already in use. Please choose another name.", "Key Name In Use",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //:> Create a new AES key
            Aes aes = _cryptingAes.CreateAES();
            string aesKey = _cryptingAes.ByteArToReadableString(aes.Key);
            string aesIV = _cryptingAes.ByteArToReadableString(aes.IV);
            string aesName = _keyName;

            string filePath = Path.Combine(Directories.KeyFolderPath, "AESInfo.txt");

            // Check if the folder exists, if not, create it
            if (!Directory.Exists(Directories.KeyFolderPath))
                Directory.CreateDirectory(Directories.KeyFolderPath);

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

            //Clear the keyName
            ClearKeyName();
        }

        private void BtnDecryptAES_Click(object sender, RoutedEventArgs e)
        {
            //Check if it's okay to decrypt
            if (!IsOkToDecrypt()) return;

            try
            {
                int indexPickedAes = ListKeys.SelectedIndex;
                int indexPickedImg = ListDecryptedImgs.SelectedIndex;
                SaveDecryptedImg(_cryptingAes.Decrypt(_encryptedImgInfo[indexPickedImg].EncryptedImg,
                    _aesList[indexPickedAes]));


                if (OpenFolder == true)
                {
                    string directoryPath = Path.GetDirectoryName(filePathA);

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        Arguments = directoryPath,
                        FileName = "explorer.exe"
                    };

                    Process.Start(startInfo);
                }
                Hashing hashing = new Hashing();
                StringBuilder sb = new StringBuilder();
                byte[] HashedSHA;
                byte[] HashedBLAKE;
                HashedSHA = hashing.FileToHashSHA(filePathA);
                HashedBLAKE = hashing.FileToHashBLAKE(filePathA);
                string SHAString1 = Convert.ToBase64String(HashedSHA);
                string BlakeString = Convert.ToBase64String(HashedBLAKE);

                sb.AppendLine("Generating hashes to check file validity:");
                sb.AppendLine("");
                sb.AppendLine($"SHA256 Hash: {SHAString1}");
                sb.AppendLine("");
                sb.AppendLine($"Blake hash: {BlakeString}");
                sb.AppendLine("");
                sb.AppendLine("Hashing complete. Please ensure these match with the original hashes to guarantee integrity.");
                MessageBox.Show(sb.ToString(), "Integrity Checker");




                System.Windows.MessageBox.Show("Decryption successful, image saved!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear the name
                _decryptedImgName = string.Empty;
                TxtDecryptedImgName.Text = "Click to Name Image";
                TxtDecryptedImgName.Foreground = System.Windows.Media.Brushes.LightSteelBlue;

                // Unselect Items in listbox'
                ListDecryptedImgs.SelectedIndex = -1;
                ListKeys.SelectedIndex = -1;
            }
            catch (CryptographicException ex)
            {
                System.Windows.MessageBox.Show(
                    $"Something went wrong while decrypting the image. Are you sure you selected the right key?\n" +
                    $"{ex.Message}",
                    "",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Reading

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();

            //openFileDialog.Filter = "Image Files (*.png;)|*.png;";
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif;";
            openFileDialog.Title = "Select an image file";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);

                BitmapImage NewImg = new BitmapImage();
                NewImg.BeginInit();
                NewImg.CacheOption = BitmapCacheOption.OnLoad;
                NewImg.StreamSource = new MemoryStream(imageBytes);
                NewImg.EndInit();

                PickedImg.Source = NewImg;
                _cryptingAes.ImageEncoded = Convert.ToBase64String(imageBytes);

                #region FileName

                string filePath = openFileDialog.FileName;
                int lastSlashIndex = Math.Max(filePath.LastIndexOf('\\'), filePath.LastIndexOf('/'));
                string result = (lastSlashIndex >= 0) ? filePath.Substring(lastSlashIndex + 1) : filePath;
                TxtPlainImgName.Text = result;

                #endregion FileName

                Console.WriteLine(_cryptingAes.ImageEncoded);
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
        }

        private void ReadEncryptedImg()
        {
            // Clear the list
            _encryptedImgInfo.Clear();
            ListDecryptedImgs.ItemsSource = null;

            //! Check if the directory exists
            if (!Directory.Exists(Directories.EncryptFolderPath)) return;

            //Get all the text files in the directory
            string[] textFiles = Directory.GetFiles(Directories.EncryptFolderPath, "*.txt");

            //Iterate over all the files
            foreach (var file in textFiles)
            {
                using (StreamReader reader = new StreamReader(file))
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

                            _encryptedImgInfo.Add(info);
                            imgName = imgBase64 = null; // Reset for the next group
                        }
                    }
                }

                // Create a list of the image names
                List<string> nameList = new List<string>();
                foreach (EncryptedImgInfo info in _encryptedImgInfo)
                {
                    nameList.Add(info.ImgName);
                }

                // Set the list as the source for the ListBox
                ListDecryptedImgs.ItemsSource = nameList;
            }
        }

        private string GetEncryptedImgName()
        {
            if (ListDecryptedImgs.SelectedIndex != -1)
                return _encryptedImgInfo[ListDecryptedImgs.SelectedIndex].ImgName;
            return string.Empty;
        }

        private void ReadAesKeyFromFile()
        {
            // Clear the list
            _aesList.Clear();
            ListKeys.ItemsSource = null;

            if (File.Exists(Path.Combine(Directories.KeyFolderPath, "AESInfo.txt")))
            {
                using (StreamReader reader = new StreamReader(Path.Combine(Directories.KeyFolderPath, "AESInfo.txt")))
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

                            _aesList.Add(info);
                            aesName = aesKey = aesIV = null; // Reset for the next group
                        }
                    }
                }

                List<string> nameList = new List<string>();
                foreach (AesInfo aesInfo in _aesList)
                {
                    nameList.Add(aesInfo.AesName);
                }

                ListKeys.ItemsSource = nameList;
            }
        }

        #endregion

        #region Writing

        private void SaveDecryptedImg(string decryptedImg)
        {
            // Decode the Base64 string to bytes
            byte[] imageBytes = Convert.FromBase64String(decryptedImg);

            // Create a MemoryStream to hold the image bytes
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                // Create an Image from the MemoryStream
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                //TODO: All Images are saved as PNG by default but images can be selected in different formats.
                var filePath = Path.Combine(Directories.DecryptedFolderPath,
                    CheckDefaultImgName.IsChecked == true
                        ? $"{_decryptedImgName}.png"
                        : $"{GetEncryptedImgName()}.png");

                filePathA = filePath;
                // Ensure that directories exist
                Directories.EnsureDirectoriesExist();

                // Save the image to the specified path
                image.Save(filePath);
            }
        }

        private void SaveEncryptedImg(string EncryptedImg)
        {
            //string folderPath = Path.Combine($"{Directories.EncryptFolderPath}\\Encrypted\\");
            string filePath = Path.Combine(Directories.EncryptFolderPath, $"{_encryptedImgName}.txt");

            Directories.EnsureDirectoriesExist();

            using (StreamWriter writer = File.AppendText(filePath))
            {
                // Write the AES information as a group of three
                writer.WriteLine($"Img Name: {_encryptedImgName}");
                writer.WriteLine($"AES-Encrypted Image: {EncryptedImg}");
                writer.WriteLine(); // Add an empty line to separate groups
            }

            ReadEncryptedImg();
        }

        #endregion

        #region Assigning

        private void TxtForNames_Click(object sender, MouseButtonEventArgs e)
        {
            // Get sender's name as string
            string senderName = ((TextBlock)sender).Name.ToLower();

            if (senderName.Contains("key")) AssignKeyName();
            else if (senderName.Contains("encrypt")) AssignEncryptedImgName();
            else if (senderName.Contains("decrypt")) AssignDecryptedImgName();
        }

        private void AssignKeyName()
        {
            // Open the InputWindow and await the user's input
            InputWindow inputWindow = new InputWindow();
            inputWindow.ShowDialog();

            // If the user didn't press OK, return
            if (!inputWindow.IsOk) return;

            // Set the keyName to the trimmed input text
            _keyName = inputWindow.InputText.Trim();

            // Display
            TxtKeyName.Text = string.IsNullOrEmpty(_keyName) ? "Click to Name Key" : _keyName;
            TxtKeyName.Foreground = string.IsNullOrEmpty(_keyName)
                ? System.Windows.Media.Brushes.LightSteelBlue
                : System.Windows.Media.Brushes.White;
        }

        private void AssignEncryptedImgName()
        {
            // Open the InputWindow and await the user's input
            InputWindow inputWindow = new InputWindow();
            inputWindow.ShowDialog();

            // If the user didn't press OK, return
            if (!inputWindow.IsOk) return;

            // Set the img name to the trimmed input text
            _encryptedImgName = inputWindow.InputText.Trim();

            // Display
            TxtEncryptedImgName.Text =
                string.IsNullOrEmpty(_encryptedImgName) ? "Click to Name Image" : _encryptedImgName;
            TxtEncryptedImgName.Foreground = string.IsNullOrEmpty(_encryptedImgName)
                ? System.Windows.Media.Brushes.LightSteelBlue
                : System.Windows.Media.Brushes.White;
        }

        private void AssignDecryptedImgName()
        {
            // Open the InputWindow and await the user's input
            InputWindow inputWindow = new InputWindow();
            inputWindow.ShowDialog();

            // If the user didn't press OK, return
            if (!inputWindow.IsOk) return;

            // Set the img name to the trimmed input text
            _decryptedImgName = inputWindow.InputText.Trim();

            // Display
            TxtDecryptedImgName.Text =
                string.IsNullOrEmpty(_decryptedImgName) ? "Click to Name Image" : _decryptedImgName;
            TxtDecryptedImgName.Foreground = string.IsNullOrEmpty(_decryptedImgName)
                ? System.Windows.Media.Brushes.LightSteelBlue
                : System.Windows.Media.Brushes.White;
        }

        #endregion

        #region Checkups

        private bool IsOkToEncrypt()
        {
            //! If no image is selected, return
            if (_cryptingAes.ImageEncoded == null)
            {
                System.Windows.MessageBox.Show("Please select an image to encrypt", "No Image", MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
                return false;
            }

            //! If no key is selected, return
            if (ListKeys.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Please select a key to encrypt the image with.", "No Key",
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }

            // Is there an image name given?
            if (string.IsNullOrEmpty(_encryptedImgName))
            {
                //? Ask if user wants to name the image now.
                var result = MessageBox.Show(
                    "No name for the to-be-encrypted image was given. Would you like to name it now?",
                    "Nameless image?",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);

                //! If user doesn't want to name the image now, return.
                if (result == MessageBoxResult.No) return false;

                //> Else, open the InputWindow and await the user's input
                AssignEncryptedImgName();

                //! If the user still didn't give a name, return. (Not again.. :\ )
                if (string.IsNullOrEmpty(_encryptedImgName)) return false;
            }

            // Green light
            return true;
        }

        private bool IsOkToDecrypt()
        {
            //! Return if no image is selected
            if (ListDecryptedImgs.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Please select an encrypted image to decrypt.", "Imageless...",
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }

            //! Return if no key is selected
            if (ListKeys.SelectedIndex == -1)
            {
                System.Windows.MessageBox.Show("Please select a key to decrypt the image with.", "Keyless...",
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }

            //! Return if no custom name was given even when prompted
            if (CheckDefaultImgName.IsChecked == true && _decryptedImgName == string.Empty)
            {
                MessageBox.Show("Please give a name to the decrypted image.", "Nameless Image",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Green light
            return true;
        }

        #endregion
    }
}