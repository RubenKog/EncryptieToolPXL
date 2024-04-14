using System;
using System.IO;
using System.Xml.Linq;

namespace KeysLibrary.Services
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
        /// Folder where the app settings are stored. App settings contain the paths to the keys and encrypted files as an XDocument.
        /// </summary>
        private static readonly string AppSettingsPath
            = Path.Combine(RootFolderPath, "Settings");

        /// <summary>
        /// Folder where the (plain text) AES keys are stored.
        /// </summary>
        public static string PlainAesPath
            = Path.Combine(RootFolderPath, "Aes_Keys");
        
        /// <summary>
        /// Folder where the (cipher text) AES keys are stored.
        /// </summary>
        public static string CipherAesPath
            = Path.Combine(RootFolderPath, "Aes_Encrypted");

        /// <summary>
        /// Folder where the (decrypted cipher text) AES keys are stored.
        /// </summary>
        public static string DecryptedCipherAesPath
            = Path.Combine(RootFolderPath, "Aes_Decrypted");

        /// <summary>
        /// Folder where the encrypted files are stored.
        /// </summary>
        public static string EncryptedImgPath
            = Path.Combine(RootFolderPath, "Images_Encrypted");

        /// <summary>
        /// Folder where the decrypted files are stored.
        /// </summary>
        public static string DecryptedImgPath
            = Path.Combine(RootFolderPath, "Images_Decrypted");

        /// <summary>
        /// Folder where the RSA private keys are stored.
        /// </summary>
        public static string RsaPrivatePath
            = Path.Combine(RootFolderPath, "RSA_Private");

        /// <summary>
        /// Folder where the RSA public keys are stored.
        /// </summary>
        public static string RsaPublicPath
            = Path.Combine(RootFolderPath, "RSA_Public");

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
            //! Return if the settings file is corrupted
            if (!VerifySettingsFile()) return false;

            //Load the settings from the file
            XDocument settings = XDocument.Load(AppSettingsPath + "/Settings.xml");

            //Get the paths from the settings
            PlainAesPath = settings.Element("Paths")?.Element("PlainAesPath")?.Value;
            CipherAesPath = settings.Element("Paths")?.Element("CipherAesPath")?.Value;
            DecryptedCipherAesPath = settings.Element("Paths")?.Element("DecryptedCipherAesPath")?.Value;
            EncryptedImgPath = settings.Element("Paths")?.Element("EncryptedImgPath")?.Value;
            DecryptedImgPath = settings.Element("Paths")?.Element("DecryptedImgPath")?.Value;
            RsaPublicPath = settings.Element("Paths")?.Element("RsaPublicPath")?.Value;
            RsaPrivatePath = settings.Element("Paths")?.Element("RsaPrivatePath")?.Value;

            // Return true to indicate that the settings were correctly loaded :]
            return true;
        }

        /// <summary>
        /// Checks if all the paths in the settings file are present.
        /// </summary>
        /// <returns>False if it misses a path.</returns>
        private static bool VerifySettingsFile()
        {
            //Do the settings file exist?
            if (!File.Exists(AppSettingsPath + "/Settings.xml")) return false;
            
            //Load the settings from the file
            var settings = XDocument.Load(AppSettingsPath + "/Settings.xml");
            
            //Check if all the paths are present
            return settings.Element("Paths")?.Element("PlainAesPath") != null &&
                   settings.Element("Paths")?.Element("CipherAesPath") != null &&
                   settings.Element("Paths")?.Element("DecryptedCipherAesPath") != null &&
                   settings.Element("Paths")?.Element("EncryptedImgPath") != null &&
                   settings.Element("Paths")?.Element("DecryptedImgPath") != null &&
                   settings.Element("Paths")?.Element("RsaPublicPath") != null &&
                   settings.Element("Paths")?.Element("RsaPrivatePath") != null;
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
                    new XElement("PlainAesPath", PlainAesPath),
                    new XElement("CipherAesPath", CipherAesPath),
                    new XElement("DecryptedCipherAesPath", DecryptedCipherAesPath),
                    new XElement("EncryptedImgPath", EncryptedImgPath),
                    new XElement("DecryptedImgPath", DecryptedImgPath),
                    new XElement("RsaPublicPath", RsaPublicPath),
                    new XElement("RsaPrivatePath", RsaPrivatePath)
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
            //App folder
            Directory.CreateDirectory(RootFolderPath);
            Directory.CreateDirectory(AppSettingsPath);
            
            //Image folders
            Directory.CreateDirectory(EncryptedImgPath);
            Directory.CreateDirectory(DecryptedImgPath);
            
            //AES folders
            Directory.CreateDirectory(PlainAesPath);
            Directory.CreateDirectory(CipherAesPath);
            Directory.CreateDirectory(DecryptedCipherAesPath);
            
            //RSA folders
            Directory.CreateDirectory(RsaPrivatePath);
            Directory.CreateDirectory(RsaPublicPath);
        }

        /// <summary>
        /// Resets the paths to the default paths and saves them to the settings.
        /// </summary>
        public static void ResetPaths()
        {
            //Reset the paths to the default paths
            PlainAesPath = Path.Combine(RootFolderPath, "Aes_Keys");
            CipherAesPath = Path.Combine(RootFolderPath, "Aes_Encrypted");
            DecryptedCipherAesPath = Path.Combine(RootFolderPath, "Aes_Decrypted");
            EncryptedImgPath = Path.Combine(RootFolderPath, "Images_Encrypted");
            DecryptedImgPath = Path.Combine(RootFolderPath, "Images_Decrypted");
            RsaPrivatePath = Path.Combine(RootFolderPath, "RSA_Private");
            RsaPublicPath = Path.Combine(RootFolderPath, "RSA_Public");

            //Save the paths to the settings
            SavePaths();
        }

        #endregion
    }
}