using KeysLibrary;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace EncryptieTool.Views
{
	public partial class RSA_First : Window
	{
		public RSA_First()
		{
			InitializeComponent();
		}

		private void BtnUseRSA_Click(object sender, RoutedEventArgs e)
		{
			// Implementatie voor het gebruik van een bestaande RSA-sleutel
			ChosenKey.Method = "Use";
			MessageBox.Show("Gebruik een bestaande RSA-sleutel", "Informatie", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void BtnGenRSA_Click(object sender, RoutedEventArgs e)
		{
			// Implementatie voor het genereren van een nieuwe RSA-sleutel
			ChosenKey.Method = "Gen";

			var rsa = new CryptingRSA();
			var (publicKey, privateKey) = rsa.GenerateRSAKeyPair();

			// Genereer een unieke bestandsnaam op basis van de huidige tijd
			string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
			string publicKeyFilename = $"RSA_PublicKey_{timestamp}.xml";
			string privateKeyFilename = $"RSA_PrivateKey_{timestamp}.xml";

			// Stel een standaardlocatie in om de sleutels op te slaan, bijv. in de map Documenten
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string fullPathPublic = Path.Combine(folderPath, publicKeyFilename);
			string fullPathPrivate = Path.Combine(folderPath, privateKeyFilename);

			// Sla de sleutels op
			File.WriteAllText(fullPathPublic, publicKey);
			File.WriteAllText(fullPathPrivate, privateKey);

			MessageBox.Show($"RSA-sleutelpaar gegenereerd en opgeslagen als:\n{fullPathPublic}\n{fullPathPrivate}", "RSA Sleutelpaar", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private string ChoosePngFile()
		{
			var openFileDialog = new Microsoft.Win32.OpenFileDialog
			{
				Filter = "PNG Files (*.png)|*.png",
				Title = "Selecteer een PNG-bestand"
			};

			bool? result = openFileDialog.ShowDialog();
			if (result == true)
			{
				return openFileDialog.FileName;
			}

			return null;
		}

		private void BtnEncryptAndSave_Click(object sender, RoutedEventArgs e)
		{
			string pngFilePath = ChoosePngFile();
			if (pngFilePath == null)
			{
				MessageBox.Show("Geen bestand geselecteerd.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Genereer RSA-sleutelpaar
			var rsa = new CryptingRSA();
			var (publicKey, privateKey) = rsa.GenerateRSAKeyPair();

			// Versleutel de inhoud van het bestand (voor de demo versleutelen we een placeholder tekst)
			var encryptedData = rsa.EncryptData($"Versleutelde inhoud van {Path.GetFileName(pngFilePath)}", publicKey);
			string encryptedContentBase64 = Convert.ToBase64String(encryptedData);

			// Definieer de map waarin je de bestanden wilt opslaan
			string folderPath = @"C:\EncryptedPNGs";
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			// Sla de versleutelde inhoud en de sleutels op
			string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
			File.WriteAllText(Path.Combine(folderPath, $"encrypted_{timestamp}.txt"), encryptedContentBase64);
			File.WriteAllText(Path.Combine(folderPath, $"publicKey_{timestamp}.xml"), publicKey);
			File.WriteAllText(Path.Combine(folderPath, $"privateKey_{timestamp}.xml"), privateKey);

			MessageBox.Show($"Versleuteling voltooid en bestanden opgeslagen in {folderPath}.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}