using EncryptieTool.Windows;
using KeysLibrary.Models;
using KeysLibrary.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace EncryptieTool.Views
{
    public partial class RsaView : Page
    {
        public RsaView()
        {
            InitializeComponent();
            ReadAllKeys();
            RefreshGui();
        }

        public ObservableCollection<AesInfo> PlainAesList = new ObservableCollection<AesInfo>();
        public ObservableCollection<RsaPublicKey> RsaPublicList = new ObservableCollection<RsaPublicKey>();
        public ObservableCollection<RsaPrivateKey> RsaPrivateList = new ObservableCollection<RsaPrivateKey>();
        public ObservableCollection<AesInfo> CipherAesList = new ObservableCollection<AesInfo>();
        private string _newKeyName = string.Empty;
        private string _cipherKeyName = string.Empty;

        private void RefreshGui()
        {
            //Refresh Textbox'
            TxtNewKeyName.Text = _newKeyName == string.Empty ? "Click to name key" : _newKeyName;
            TxtNewKeyName.Foreground = _newKeyName == string.Empty
                ? System.Windows.Media.Brushes.LightSteelBlue
                : System.Windows.Media.Brushes.White;
            TxtCipherAes.Text = _cipherKeyName == string.Empty ? "Click to name cipher key" : _cipherKeyName;
            TxtCipherAes.Foreground = _cipherKeyName == string.Empty
                ? System.Windows.Media.Brushes.LightSteelBlue
                : System.Windows.Media.Brushes.White;
        }

        private void ReadAllKeys()
        {
            //Clear lists
            RsaPublicList.Clear();
            LstPublicRsa.Items.Clear();
            PlainAesList.Clear();
            LstPlainAes.Items.Clear();

            //Read
            ReadPlainAesKeys();
            ReadPublicRsa();
            ReadPrivateRsaKeys();
            ReadCipherAesKeys();
        }

        private void BtnUseRSA_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\RSAPrivate",
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Select a Private RSA Key"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string privateKey = File.ReadAllText(openFileDialog.FileName);

                try
                {
                    string selectedAesName = LstPlainAes.SelectedItem.ToString();
                    AesInfo selectedAesInfo = PlainAesList.FirstOrDefault(aes => aes.AesName == selectedAesName);

                    if (selectedAesInfo == null)
                    {
                        MessageBox.Show("Select a valid AES key from the list", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    string encryptedAesKeyPath =
                        Path.Combine(@"C:\RSAPublic", $"{selectedAesInfo.AesName}_EncryptedAESKey.txt");
                    if (!File.Exists(encryptedAesKeyPath))
                    {
                        MessageBox.Show("Encrypted AES Key file not found", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    byte[] encryptedAesKey = File.ReadAllBytes(encryptedAesKeyPath);
                    var decryptedAesKey = RsaService.DecryptData(encryptedAesKey, privateKey);

                    TxtDecryptedAES.Text = decryptedAesKey;
                }
                catch (CryptographicException)
                {
                    MessageBox.Show("The selected RSA Key is not correct or the file is damaged",
                        "Decryption Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnEncryptClick(object sender, RoutedEventArgs e)
        {
            //! Check if an Aes and public rsa pair are selected
            if (LstPlainAes.SelectedItem == null || LstPublicRsa.SelectedItem == null)
            {
                MessageBox.Show("Please select an Aes and Rsa key pair!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //? Check if the user has named the cipher key
            if (_cipherKeyName == string.Empty)
            {
                MessageBox.Show("Please name the cipher key!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Get selected keys
            //var item = ((ListBoxItem)LstPublicRsa.SelectedItem).Content;
            RsaPublicKey rsaKey = RsaPublicList.FirstOrDefault(x => x.FileName == (string)((ListBoxItem)LstPublicRsa.SelectedItem).Content);
            AesInfo aesKey = PlainAesList.FirstOrDefault(x => x.AesName == (string)((ListBoxItem)LstPlainAes.SelectedItem).Content);

            //! Check if keys are valid
            if (aesKey == null || rsaKey == null)
            {
                MessageBox.Show("Please select a valid Aes and Rsa key pair!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Encrypt the AES key
            var cipherAes = RsaService.Encrypt(aesKey, rsaKey);

            //Save the encrypted AES key
            SaveCipherAes(cipherAes);
        }

        private void SaveCipherAes(byte[] cipher)
        {
            //Save the cipher
            Directories.EnsureDirectoriesExist();
            File.WriteAllBytes(Path.Combine(Directories.CipherAesPath, $"Cipher-{_cipherKeyName}.txt"), cipher);

            //Display
            MessageBox.Show("AES key encrypted and saved!", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void SaveData(byte[] encryptedAesKey, string publicKey, string privateKey, string aesName)
        {
            string publicPath = @"C:\RSAPublic";
            string privatePath = @"C:\RSAPrivate";
            Directory.CreateDirectory(publicPath);
            Directory.CreateDirectory(privatePath);

            File.WriteAllText(Path.Combine(publicPath, $"{aesName}_PublicKey.xml"), publicKey);
            File.WriteAllText(Path.Combine(privatePath, $"{aesName}_PrivateKey.xml"), privateKey);
            File.WriteAllBytes(Path.Combine(publicPath, $"{aesName}_EncryptedAESKey.txt"), encryptedAesKey);

            MessageBox.Show("Encryptie en opslag voltooid.", "Succes", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ReadPlainAesKeys()
        {
            //Prep
            PlainAesList.Clear();
            string path = Path.Combine(Directories.PlainAesPath, "AESInfo.txt");

            //! Return if file does not exist
            if (!File.Exists(path)) return;

            using (var reader = new StreamReader(path))
            {
                //Prep
                string line;
                var info = new AesInfo();

                //Read
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("AES Name:"))
                        info.AesName = line.Split(':')[1].Trim();
                    else if (line.StartsWith("AES Key:"))
                        info.AesKey = line.Split(':')[1].Trim();
                    else if (line.StartsWith("AES IV:"))
                        info.AesIV = line.Split(':')[1].Trim();
                    else if (string.IsNullOrWhiteSpace(line)
                             && info.AesName != null && info.AesKey != null && info.AesIV != null)
                    {
                        //Add to list
                        PlainAesList.Add(info);
                        LstPlainAes.Items.Add(new ListBoxItem() { Content = info.AesName });

                        //Reset
                        info = new AesInfo();
                    }
                }
            }
        }

        private void ReadPublicRsa()
        {
            //Prep
            RsaPublicList.Clear();

            //Ensure directory exists
            Directories.EnsureDirectoriesExist();

            //Read all XML files and determine whether they are public keys
            var files = Directory.GetFiles(Directories.RsaPublicPath, "*.xml");
            foreach (var file in files)
            {
                // Skip file if it is not a public key
                if (!IsFilePublicKey(file)) continue;

                //Load XML document
                XDocument doc = XDocument.Load(file);

                //Create new RSA public key model
                var key = new RsaPublicKey(
                    doc.Element("RSAKeyValue")?.Element("Modulus")?.Value,
                    doc.Element("RSAKeyValue")?.Element("Exponent")?.Value,
                    file);

                // Add to list
                RsaPublicList.Add(key);
                LstPublicRsa.Items.Add(new ListBoxItem() { Content = key.FileName });
            }
        }

        private void ReadPrivateRsaKeys()
        {
            LstPrivateRsa.Items.Clear();

            string rsaPrivatePath = Directories.RsaPrivatePath;
            if (!Directory.Exists(rsaPrivatePath))
            {
                Console.WriteLine($"The directory for the RSA Private Key doesn't exist: {rsaPrivatePath}");
                return;
            }

            var privateKeyFiles = Directory.GetFiles(rsaPrivatePath, "*.xml");

            foreach (string file in privateKeyFiles)
            {
                try
                {
                    XDocument doc = XDocument.Load(file);
                    var rsaKey = new RsaPrivateKey(
                        modulus: doc.Root.Element("Modulus")?.Value,
                        exponent: doc.Root.Element("Exponent")?.Value,
                        d: doc.Root.Element("D")?.Value,
                        p: doc.Root.Element("P")?.Value,
                        q: doc.Root.Element("Q")?.Value,
                        dp: doc.Root.Element("DP")?.Value,
                        dq: doc.Root.Element("DQ")?.Value,
                        inverseQ: doc.Root.Element("InverseQ")?.Value,
                        filePath: file
                    );

                    if (rsaKey.Modulus != null && rsaKey.Exponent != null && rsaKey.D != null)
                    {
                        ListBoxItem item = new ListBoxItem();
                        item.Content = rsaKey.FileName;
                        item.Tag = rsaKey;
                        LstPrivateRsa.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with reading Private RSA Key from file: {file}: {ex.Message}");
                }
            }
        }

        private void ReadCipherAesKeys()
        {
            CipherAesList.Clear();
            LstCipherAes.Items.Clear();

            string path = Directories.CipherAesPath;
            if (!Directory.Exists(path))
            {
                MessageBox.Show($"The directory for the Cipher AES doesn't exist: {path}");
                return;
            }

            var cipherFiles = Directory.GetFiles(path, "*.txt");

            foreach (var file in cipherFiles)
            {
                string aesName = Path.GetFileNameWithoutExtension(file);
                var aesInfo = new AesInfo { AesName = aesName };
                aesInfo.AesKey = File.ReadAllText(file);
                CipherAesList.Add(aesInfo);
                LstCipherAes.Items.Add(new ListBoxItem { Content = aesInfo });
            }
        }

        /// <summary>
        /// Does a basic check, based on xml element names, to determine whether a file is a public RSA key.
        /// </summary>
        private bool IsFilePublicKey(string path)
        {
            try
            {
                //Load XML document
                XDocument doc = XDocument.Load(path);
                XElement root = doc.Root;

                //Check
                return root.Name == "RSAKeyValue" && root.Element("Modulus") != null &&
                       root.Element("Exponent") != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private void InputField_Click(object sender, MouseButtonEventArgs e)
        {
            // Get name of sender
            string name = ((TextBlock)sender).Name;

            // Get user input
            string input = GetUserInput();

            //! Return if cancelled
            if (input == "@CANCEL@") return;

            // Assign
            if (name.Contains("NewKey"))
                _newKeyName = input;
            else if (name.Contains("CipherAes"))
                _cipherKeyName = input;

            // Refresh
            RefreshGui();
        }

        /// <summary>
        /// Opens a new window to get user input.
        /// </summary>
        /// <returns>User input. If not confirmed, returns @CANCEL@</returns>
        private string GetUserInput()
        {
            //Create new window
            InputWindow inputWindow = new InputWindow();
            inputWindow.ShowDialog();

            //If not confirmed, return
            if (!inputWindow.IsOk) return "@CANCEL@";

            //Return input
            return inputWindow.InputText.Trim();
        }

        private void ButtonCreateKeys(object sender, RoutedEventArgs e)
        {
            //Create
            var (publicKey, privateKey) = RsaService.GenerateRsaKeyPair();

            //Save
            File.WriteAllText(Path.Combine(Directories.RsaPublicPath, $"Public-{_newKeyName}.xml"), publicKey);
            File.WriteAllText(Path.Combine(Directories.RsaPrivatePath, $"Private-{_newKeyName}.xml"), privateKey);

            //Display
            RefreshGui();
            MessageBox.Show("Keys successfully created!", "Success", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ButtonDecrypt(object sender, RoutedEventArgs e)
        {
            if (LstCipherAes.SelectedItem == null || LstPrivateRsa.SelectedItem == null)
            {
                MessageBox.Show("Select the correct Cipher AES Key and correct Private RSA Key", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var selectedCipherAes = (AesInfo)((ListBoxItem)LstCipherAes.SelectedItem).Content;
                var selectedPrivateKey = (RsaPrivateKey)((ListBoxItem)LstPrivateRsa.SelectedItem).Tag;

                string encryptedAesKeyPath = Path.Combine(Directories.CipherAesPath, $"{selectedCipherAes.AesName}.txt");
                byte[] encryptedAesKey = File.ReadAllBytes(encryptedAesKeyPath);
                string decryptedAesKey = RsaService.DecryptData(encryptedAesKey, selectedPrivateKey.ToXmlString());

                MessageBox.Show($"Decryption succesfull. Decryoted key: {decryptedAesKey}", "Decryption succesfull", MessageBoxButton.OK, MessageBoxImage.Information);

                string outputFilePath = Path.Combine(Directories.DecryptedCipherAesPath, $"{selectedCipherAes.AesName}_Decrypted.txt");
                File.WriteAllText(outputFilePath, decryptedAesKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during the decryption process: {ex.Message}", "Decryption Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}