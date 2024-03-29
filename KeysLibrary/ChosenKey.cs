using System;
using System.IO;
using System.Security.Permissions;

namespace KeysLibrary
{
	public static class ChosenKey
	{
		//string with value RSA or AES
		public static string KeyType { get; set; } //Wordt niet eens gebruikt, alleen assigned.

		//Use or Generate key
		public static string Method { get; set; }

		//folder location
		public static string FilePath { get; set; } = Path.Combine(Environment.SpecialFolder.MyDocuments.ToString(), "HOMEWORK_2PROA1");
	}
}