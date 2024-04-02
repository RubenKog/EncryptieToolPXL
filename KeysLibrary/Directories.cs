using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace KeysLibrary
{
    public static class Directories
    {
        #region Properties

        /// <summary>
        /// Root Folder where all the files are stored by default.
        /// Will also save app settings.
        /// </summary>
        public static readonly string RootFolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HOMEWORK_2PROA1");

        /// <summary>
        /// Folder where the keys are stored.
        /// </summary>
        public static string KeyFolderPath
            = Path.Combine(RootFolderPath, "Keys");

        /// <summary>
        /// Folder where the encrypted files are stored.
        /// </summary>
        public static string EncryptFolderPath
            = Path.Combine(RootFolderPath, "Encrypted");

        /// <summary>
        /// Folder where the decrypted files are stored.
        /// </summary>
        public static string DecryptedFolderPath
            = Path.Combine(RootFolderPath, "Decrypted");

        /// <summary>
        /// Folder where the app settings are stored. App settings contain the paths to the keys and encrypted files as an XDocument.
        /// </summary>
        public static readonly string AppSettingsPath
            = Path.Combine(RootFolderPath, "Settings");

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the directories and creates an app settings file if it doesn't exist.
        /// </summary>
        public static void Initialize()
        {
            //Create the directories if they don't exist
            EnsureDirectoriesExist();

            //Create and save default paths settings if they don't exist
            if (!ReadPathsFromSettings()) SavePaths();
        }

        /// <summary>
        /// Reads the paths from the settings file.
        /// </summary>
        /// <returns>False if no settings file was found.</returns>
        private static bool ReadPathsFromSettings()
        {
            //! Check if the settings file exists else return false
            if (!File.Exists(AppSettingsPath + "/Settings.xml")) return false;

            //Load the settings from the file
            XDocument settings = XDocument.Load(AppSettingsPath + "/Settings.xml");

            //Get the paths from the settings
            KeyFolderPath = settings.Element("Paths").Element("KeyFolderPath").Value;
            EncryptFolderPath = settings.Element("Paths").Element("EncryptFolderPath").Value;
            DecryptedFolderPath = settings.Element("Paths").Element("DecryptedFolderPath").Value;

            // Return true to indicate that the settings were loaded :]
            return true;
        }

        /// <summary>
        /// Saves the paths to the settings file based on the current paths.
        /// If the user didn't define any, it will save the default paths.
        /// </summary>
        public static void SavePaths()
        {
            //Create XDocument with the settings
            XDocument settings = new XDocument(
                new XElement("Paths",
                    new XElement("KeyFolderPath", KeyFolderPath),
                    new XElement("EncryptFolderPath", EncryptFolderPath),
                    new XElement("DecryptedFolderPath", DecryptedFolderPath)
                )
            );

            //Save the settings to the file as an xml file
            EnsureDirectoriesExist();
            settings.Save(AppSettingsPath + "/Settings.xml");
        }

        /// <summary>
        /// Creates the directories if they don't exist.
        /// </summary>
        public static void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(RootFolderPath);
            Directory.CreateDirectory(AppSettingsPath);
            Directory.CreateDirectory(EncryptFolderPath);
            Directory.CreateDirectory(DecryptedFolderPath);
            Directory.CreateDirectory(KeyFolderPath);
        }
        
        public static void ResetPaths()
        {
            //Reset the paths to the default paths
            KeyFolderPath = Path.Combine(RootFolderPath, "Keys");
            EncryptFolderPath = Path.Combine(RootFolderPath, "Encrypted");
            DecryptedFolderPath = Path.Combine(RootFolderPath, "Decrypted");

            //Save the paths to the settings
            SavePaths();
        }

        #endregion
    }
}