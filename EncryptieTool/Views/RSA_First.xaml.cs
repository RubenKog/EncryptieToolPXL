using KeysLibrary;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using Path = System.IO.Path;

namespace EncryptieTool.Views
{
	public partial class RSA_First : Window
	{
		private List<AesInfo> AesList;

		public RSA_First()
		{
			AesList = new List<AesInfo>();
			InitializeComponent();
		}

		private void GoToMainWindow(object sender, RoutedEventArgs e)
		{
			MainWindow.CurrentInstance.Show();
			this.Close();
		}

		private void BtnUseRSA_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				InitialDirectory = @"C:\RSAPrivate",
				Filter = "XML Files (*.xml)|*.xml",
				Title = "Selecteer een Private RSA Sleutel"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				string privateKey = File.ReadAllText(openFileDialog.FileName);

				try
				{
					string selectedAesName = LstAesKeys.SelectedItem.ToString();
					AesInfo selectedAesInfo = AesList.FirstOrDefault(aes => aes.AesName == selectedAesName);

					if (selectedAesInfo == null)
					{
						MessageBox.Show("Selecteer een geldige AES sleutel uit de lijst.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					string encryptedAesKeyPath = Path.Combine(@"C:\RSAPublic", $"{selectedAesInfo.AesName}_EncryptedAESKey.txt");
					if (!File.Exists(encryptedAesKeyPath))
					{
						MessageBox.Show("Geëncrypteerde AES sleutelbestand niet gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					byte[] encryptedAesKey = File.ReadAllBytes(encryptedAesKeyPath);
					var rsa = new CryptingRSA();
					var decryptedAesKey = rsa.DecryptData(encryptedAesKey, privateKey);

					TxtDecryptedAES.Text = decryptedAesKey;
				}
				catch (CryptographicException)
				{
					MessageBox.Show("De gekozen RSA sleutel is niet correct of het bestand is beschadigd.", "Decryptie Fout", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void BtnSelectAESFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Text Files (*.txt)|*.txt",
				Title = "Selecteer een AESInfo bestand"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				ReadAesKeyFromFile(openFileDialog.FileName);
			}
		}

		private void BtnEncryptAndSave_Click(object sender, RoutedEventArgs e)
		{
			if (LstAesKeys.SelectedItem == null)
			{
				MessageBox.Show("Selecteer een AES sleutel.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			string selectedAesName = LstAesKeys.SelectedItem.ToString();
			AesInfo selectedAesInfo = AesList.FirstOrDefault(aes => aes.AesName == selectedAesName);

			if (selectedAesInfo == null)
			{
				MessageBox.Show("AES sleutel niet gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var rsa = new CryptingRSA();
			var (publicKey, privateKey) = rsa.GenerateRSAKeyPair();

			var encryptedAesKey = rsa.EncryptData(selectedAesInfo.AesKey, publicKey);

			SaveData(encryptedAesKey, publicKey, privateKey, selectedAesInfo.AesName);
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

			MessageBox.Show("Encryptie en opslag voltooid.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void ReadAesKeyFromFile(string selectedFilePath)
		{
			AesList.Clear();

			using (StreamReader reader = new StreamReader(selectedFilePath))
			{
				string line;
				string aesName = null, aesKey = null, aesIV = null;

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
						aesName = aesKey = aesIV = null;
					}
				}
			}

			List<string> nameList = new List<string>();
			foreach (AesInfo aesInfo in AesList)
			{
				nameList.Add(aesInfo.AesName);
			}
			LstAesKeys.ItemsSource = nameList;
		}
	}
}