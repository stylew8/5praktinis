using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using wpf5.Encryption;

namespace wpf5
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string PasswordFilePath = "Passwords.csv";
        private static readonly string EncryptedFilePath = "Encrypted_Passwords.enc";
        private static readonly string MasterPassword = "7777"; 

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DecryptPasswordFile();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            EncryptPasswordFile();
        }

        private void DecryptPasswordFile()
        {
            if (File.Exists(EncryptedFilePath))
            {
                try
                {
                    var encryptedData = File.ReadAllBytes(EncryptedFilePath);
                    var decryptedData = new AESEncryptor().DecryptBytes(encryptedData, MasterPassword);
                    File.WriteAllBytes(PasswordFilePath, decryptedData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to decrypt the password file: {ex.Message}");
                }
            }
        }

        private void EncryptPasswordFile()
        {

                try
                {
                    var encryptor = new AESEncryptor();
                    var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                    var csvFiles = Directory.GetFiles(directoryPath, "*.csv");

                    foreach (var csvFilePath in csvFiles)
                    {
                        var data = File.ReadAllBytes(csvFilePath);
                        var encryptedData = encryptor.EncryptBytes(data, MasterPassword);
                        var encryptedFilePath = Path.Combine(Path.GetDirectoryName(csvFilePath), Path.GetFileNameWithoutExtension(csvFilePath) + ".enc");
                        File.WriteAllBytes(encryptedFilePath, encryptedData);
                        File.Delete(csvFilePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to encrypt the password file: {ex.Message}");
                }
            
        }

    }

}
