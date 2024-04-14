using System.IO;
using System.Security.Cryptography;
using System.Text;
using Blake3_Arctium;

namespace KeysLibrary.Services
{
    public class Hashing
    {

        public byte[] FileToHashSHA(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (SHA256 mySHA256 = SHA256.Create())
                {

                    try
                    {
                        // Open the file stream.
                        using (FileStream fileStream = File.OpenRead(filePath))
                        {
                            // Compute the hash of the fileStream.
                            byte[] hashValue = mySHA256.ComputeHash(fileStream);
                            return hashValue;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        //documentation -> https://github.com/NeuroXiq/Arctium
        public byte[] FileToHashBLAKE(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        BLAKE3 blake3 = new BLAKE3();
                        byte[] byteArray = Encoding.UTF8.GetBytes(filePath);
                        blake3.HashBytes(byteArray);
                        byte[] computedHash = blake3.HashFinal();
                        return computedHash;
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}
