using System.Security.Cryptography;
using System.Text;

namespace KeysLibrary
{
	public class CryptingRSA
	{
		private readonly int KeySize = 2048;

		public (string publicKey, string privateKey) GenerateRSAKeyPair()
		{
			using (var rsa = new RSACryptoServiceProvider(KeySize))
			{
				rsa.PersistKeyInCsp = false;
				var publicKey = rsa.ToXmlString(false);
				var privateKey = rsa.ToXmlString(true);
				return (publicKey, privateKey);
			}
		}

		public byte[] EncryptData(string dataToEncrypt, string publicKey)
		{
			using (var rsa = new RSACryptoServiceProvider(KeySize))
			{
				rsa.PersistKeyInCsp = false;
				rsa.FromXmlString(publicKey);
				var encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(dataToEncrypt), true);
				return encryptedData;
			}
		}

		public string DecryptData(byte[] dataToDecrypt, string privateKey)
		{
			using (var rsa = new RSACryptoServiceProvider(KeySize))
			{
				rsa.PersistKeyInCsp = false;
				rsa.FromXmlString(privateKey);
				var decryptedData = rsa.Decrypt(dataToDecrypt, true);
				return Encoding.UTF8.GetString(decryptedData);
			}
		}
	}
}