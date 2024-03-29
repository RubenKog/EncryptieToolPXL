using KeysLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace EncryptieTool.Views
{
	/// <summary>
	/// Interaction logic for AesView.xaml
	/// </summary>
	public partial class AesView : Page
	{
		#region Properties

		private string B64Text = string.Empty;
		private CryptingAES cryptingAES;
		private bool ImageSelected;
		private List<AesInfo> AesList;
		private List<EncryptedImgInfo> EncryptedImgInfo;

		#endregion


		public AesView()
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
					try
					{
						SaveCryptedImg(cryptingAES.Encrypt(cryptingAES.ImageEncoded, AesList[IndexPicked]));
						System.Windows.MessageBox.Show("Image succesfully encrypted.");
					}
					catch
					{
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

			string folderPath = Path.Combine($"{SelectedPaths.SelectedSaveEncryptFolder}\\Encrypted\\");
			string filePath = Path.Combine(folderPath, $"{TxTDecryptedTxtName.Text}.txt");

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

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
			string filePath = Path.Combine($"{SelectedPaths.SelectedSaveEncryptFolder}\\Encrypted\\EncryptedImg.txt");
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
				string folderPath = Path.Combine($"{SelectedPaths.SelectedSaveEncryptFolder}\\Decrypted\\");
				string filePath = Path.Combine(folderPath, $"{TxtImgName.Text}.png");

				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}

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
				cryptingAES.ImageEncoded = Convert.ToBase64String(imageBytes);

				#region FileName

				string filePath = openFileDialog.FileName;
				int lastSlashIndex = Math.Max(filePath.LastIndexOf('\\'), filePath.LastIndexOf('/'));
				string result = (lastSlashIndex >= 0) ? filePath.Substring(lastSlashIndex + 1) : filePath;
				lbImageName.Content = result;

				#endregion FileName

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

			string folderPath = Path.Combine(SelectedPaths.SelectedKeyFolder, "AES");
			string filePath = Path.Combine(folderPath, "AESInfo.txt");

			// Check if the folder exists, if not, create it
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

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
			string filePath = Path.Combine(SelectedPaths.SelectedKeyFolder, "Aes/AESInfo.txt");
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
				foreach (AesInfo aesInfo in AesList)
				{
					nameList.Add(aesInfo.AesName);
				}
				TxtEncrypt.ItemsSource = nameList;
			}
		}

		private void TxtEncrypt_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (TxtEncrypt.SelectedIndex == -1)
			{
				BtnEncryptAESx.IsEnabled = false;
			}
			else if (ImageSelected == true)
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