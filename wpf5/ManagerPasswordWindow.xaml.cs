using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using wpf5.Encryption;
using wpf5.Manager;
using wpf5.Models;

namespace wpf5
{
    /// <summary>
    /// Interaction logic for ManagerPasswordWindow.xaml
    /// </summary>
    public partial class ManagerPasswordWindow : Window
    {
        private string password = "";
        private string Username = String.Empty;

        public ManagerPasswordWindow(string username, PasswordBox pass)
        {
            InitializeComponent();
            password = pass.Password;
            Username = username;
        }

        private void LoadAllPasswordsButton_Click(object sender, RoutedEventArgs e)
        {
            string masterPassword = password;

            if (string.IsNullOrEmpty(masterPassword))
            {
                MessageBox.Show("Master password cannot be empty.");
                return;
            }

            var allPasswords = new PasswordManager().GetAllPasswords(Username);

            if (allPasswords.Count > 0)
            {
                PasswordGrid.ItemsSource = allPasswords.Select(result => new PasswordEntry
                {
                    Title = result[0],
                    EncryptedPassword = result[1],
                    Url = result[2],
                    Comment = result[3],
                    DecryptedPassword = ""
                }).ToList();
            }
            else
            {
                MessageBox.Show("No passwords found.");
                PasswordGrid.ItemsSource = null;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string title = SearchTitleBox.Text;
            string masterPassword = password;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(masterPassword))
            {
                MessageBox.Show("Title and master password cannot be empty.");
                return;
            }

            var searchResults = new PasswordManager().SearchPassword(title, masterPassword,Username);

            if (searchResults.Count > 0)
            {
                PasswordGrid.ItemsSource = searchResults.Select(result => new PasswordEntry
                {
                    Title = result[0],
                    EncryptedPassword = result[1],
                    Url = result[2],
                    Comment = result[3],
                    DecryptedPassword = ""
                }).ToList();
            }
            else
            {
                MessageBox.Show("No password found with the given title.");
                PasswordGrid.ItemsSource = null;
            }
        }

        private void AddPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var addPasswordWindow = new AddPasswordFormWindow();
            if (addPasswordWindow.ShowDialog() == true)
            {
                string title = addPasswordWindow.TitleBox.Text;
                string password = addPasswordWindow.PasswordBox.Password;
                string url = addPasswordWindow.UrlBox.Text;
                string comment = addPasswordWindow.CommentBox.Text;
                new PasswordManager().SavePassword(title, password, url, comment, this.password,Username);
                MessageBox.Show("Password added successfully.");
                RefreshDataGrid();
            }
        }

        private void UpdatePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string title = SearchTitleBox.Text;
            string newPassword = NewPasswordBox.Text;
            string masterPassword = this.password;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(masterPassword))
            {
                MessageBox.Show("Title, new password, and master password cannot be empty.");
                return;
            }

            new PasswordManager().UpdatePassword(title, newPassword, masterPassword, Username);
            MessageBox.Show("Password updated successfully.");
            RefreshDataGrid();
        }

        private void DeletePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string title = SearchTitleBox.Text;

            new PasswordManager().DeletePassword(title, Username);
            MessageBox.Show("Password deleted successfully.");
            RefreshDataGrid();
        }

        private void GenerateRandomPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string randomPassword = new PasswordManager().GenerateRandomPassword();
            Clipboard.SetText(randomPassword);
            MessageBox.Show("Random password generated and copied to clipboard.");
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                MessageBox.Show("Button is null.");
                return;
            }

            var encryptedPassword = button.Tag as string;
            if (string.IsNullOrEmpty(encryptedPassword))
            {
                MessageBox.Show("Encrypted password is null or empty.");
                return;
            }

            string masterPassword = this.password;

            if (string.IsNullOrEmpty(masterPassword))
            {
                MessageBox.Show("Master password cannot be empty.");
                return;
            }

            try
            {
                var decryptedPassword = new AESEncryptor().DecryptString(encryptedPassword, masterPassword);

                var passwordEntry = button.DataContext as PasswordEntry;
                if (passwordEntry != null)
                {
                    passwordEntry.DecryptedPassword = decryptedPassword;
                    PasswordGrid.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to decrypt the password: {ex.Message}. Please make sure you entered the correct master password.");
            }
        }

        private void CopyPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                MessageBox.Show("Button is null.");
                return;
            }

            var passwordEntry = button.DataContext as PasswordEntry;
            if (passwordEntry == null || string.IsNullOrEmpty(passwordEntry.DecryptedPassword))
            {
                MessageBox.Show("No decrypted password available to copy.");
                return;
            }

            Clipboard.SetText(passwordEntry.DecryptedPassword);
            MessageBox.Show("Password copied to clipboard.");
        }

        private void RefreshDataGrid()
        {
            string title = SearchTitleBox.Text;
            string masterPassword = this.password;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(masterPassword))
            {
                //MessageBox.Show("Title and master password cannot be empty.");
                return;
            }

            var searchResults = new PasswordManager().SearchPassword(title, masterPassword, Username);

            if (searchResults.Count > 0)
            {
                PasswordGrid.ItemsSource = searchResults.Select(result => new PasswordEntry
                {
                    Title = result[0],
                    EncryptedPassword = result[1],
                    Url = result[2],
                    Comment = result[3],
                    DecryptedPassword = ""
                }).ToList();
            }
            else
            {
                //MessageBox.Show("No password found with the given title.");
                PasswordGrid.ItemsSource = null;
            }
        }
    }
}
