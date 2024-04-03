using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace KeysLibrary
{
    public class Hashing
    {

        public byte[] FileToHash(string filePath)
        {
            if(File.Exists(filePath)) 
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
                            // Write the name and hash value of the file to the console.
                            Console.Write($"{Path.GetFileName(filePath)}: ");
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








    }
}
