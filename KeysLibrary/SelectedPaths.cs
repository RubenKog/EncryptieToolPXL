using System;
using System.IO;

namespace KeysLibrary
{
	public static class SelectedPaths
	{
		public static string SelectedImg;
		public static string SelectedKeyFolder 
			= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HOMEWORK_2PROA1", "Keys");
		public static string SelectedSaveEncryptFolder 
			= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HOMEWORK_2PROA1", "Encrypted");
	}
}