using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeysLibrary
{
    public static class ChosenKey
    {
        //string with value RSA or AES
        public static string KeyType { get; set; }

        //Use or Generate key
        public static string Method { get; set; }

        //folder location
        public static string FilePath { get; set; }
    }
}
