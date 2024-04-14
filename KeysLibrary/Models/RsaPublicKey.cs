using System.IO;

namespace KeysLibrary.Models
{
    public class RsaPublicKey
    {
        public string Modulus { get; set; }
        public string Exponent { get; set; }
        public string FilePath { get; set; }
        public string FileName => Path.GetFileNameWithoutExtension(FilePath);

        public RsaPublicKey(string modulus, string exponent, string filePath)
        {
            Modulus = modulus;
            Exponent = exponent;
            FilePath = filePath;
        }

        public override string ToString() => FileName;
    }
}