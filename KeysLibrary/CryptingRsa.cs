using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeysLibrary
{
    public static class CryptingRsa
    {
        public static RSA GenerateRsaKey()
        {
            //Create a public and private key
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    //Extract public key
                    RSAParameters publicKey = rsa.ExportParameters(false);
                    //Extract private key
                    RSAParameters privateKey = rsa.ExportParameters(true);
                    //Create a new RSA object
                    RSA rsaKey = RSA.Create();
                    //Import the public key
                    rsaKey.ImportParameters(publicKey);
                    //Import the private key
                    rsaKey.ImportParameters(privateKey);

                    return rsaKey;
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }
        
        public static void EncryptImage(string path, RSA rsaKey)
        {
            
        }
    }
}