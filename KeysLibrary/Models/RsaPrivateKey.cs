using System;
using System.IO;
using System.Security.Cryptography;

namespace KeysLibrary.Models
{
    public class RsaPrivateKey
    {
        public string Modulus { get; set; }
        public string Exponent { get; set; }
        public string D { get; set; } // De private exponent
        public string P { get; set; } // Eerste prime factor
        public string Q { get; set; } // Tweede prime factor
        public string DP { get; set; } // D mod (p-1)
        public string DQ { get; set; } // D mod (q-1)
        public string InverseQ { get; set; } // (Inverse of q) mod p
        public string FilePath { get; set; }
        public string FileName => Path.GetFileNameWithoutExtension(FilePath);

        public RsaPrivateKey(string modulus, string exponent, string d, string p, string q, string dp, string dq, string inverseQ, string filePath)
        {
            Modulus = modulus;
            Exponent = exponent;
            D = d;
            P = p;
            Q = q;
            DP = dp;
            DQ = dq;
            InverseQ = inverseQ;
            FilePath = filePath;
        }

        public string ToXmlString()
        {
            var rsaParams = new RSAParameters
            {
                Modulus = Convert.FromBase64String(Modulus),
                Exponent = Convert.FromBase64String(Exponent),
                D = Convert.FromBase64String(D),
                P = Convert.FromBase64String(P),
                Q = Convert.FromBase64String(Q),
                DP = Convert.FromBase64String(DP),
                DQ = Convert.FromBase64String(DQ),
                InverseQ = Convert.FromBase64String(InverseQ)
            };

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                return rsa.ToXmlString(true);
            }
        }

        public override string ToString() => FileName;
    }
}