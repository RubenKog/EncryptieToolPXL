using System;
using System.Security.Cryptography;
using System.Text;
using KeysLibrary.Models;

namespace KeysLibrary.Services
{
	public static class RsaService
	{
		private static readonly int KeySize = 2048;

		public static (string publicKey, string privateKey) GenerateRsaKeyPair()
		{
			using (var rsa = new RSACryptoServiceProvider(KeySize))
			{
				rsa.PersistKeyInCsp = false;
				var publicKey = rsa.ToXmlString(false);
				var privateKey = rsa.ToXmlString(true);
				return (publicKey, privateKey);
			}
		}

		public static byte[] Encrypt(AesInfo aesKey, RsaPublicKey rsaKey)
		{
			//! Ensure the RSA key and AES key are valid
			if (aesKey == null || rsaKey == null)
				throw new ArgumentNullException("AES key or RSA key is null.");

			// Construct the RSA public key from modulus and exponent
			using (var rsa = new RSACryptoServiceProvider(2048)) // Match the key size with what's used in RsaPublicKey
			{
				rsa.PersistKeyInCsp = false;

				// Create parameters and load them into the RSA object
				var rsaParameters = new RSAParameters
				{
					Modulus = Convert.FromBase64String(rsaKey.Modulus),
					Exponent = Convert.FromBase64String(rsaKey.Exponent)
				};
				rsa.ImportParameters(rsaParameters);

				// Encrypt the AES key
				return rsa.Encrypt(Encoding.UTF8.GetBytes(aesKey.AesKey), true);
			}
		}

		public static string DecryptData(byte[] dataToDecrypt, string privateKey)
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