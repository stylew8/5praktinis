using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using wpf5.Encryption;

namespace wpf5.UserAuth
{
    public class UserAuth : IUserAuth
    {
        private static readonly string UserFilePath = "user_credentials.txt";

        public bool RegisterUser(string username, string password)
        {
            try
            {
                var hashedPassword = HashPassword(password);

                if (!File.Exists(UserFilePath))
                {
                    File.WriteAllText(UserFilePath, $"{username}:{hashedPassword};");

                }
                else
                {
                    var x = File.ReadAllText(UserFilePath);

                    x = x + $"{username}:{hashedPassword};";

                    File.WriteAllText(UserFilePath, x);
                }

                File.Create($"{username}.enc");

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool LoginUser(string username, string password)
        {
            if (!File.Exists(UserFilePath)) return false;

            bool succ = false;

            var storedCredentials = File.ReadAllText(UserFilePath).Split(';');

            foreach (var x in storedCredentials)
            {
                var details = x.Split(':');
                if (details[0]==username)
                {
                    return VerifyPassword(details[0],password, details[1]);
                }
            }
            return false;
        }

        private string HashPassword(string password)
        {
            using var rfc2898 = new Rfc2898DeriveBytes(password, 16, 10000);
            var salt = rfc2898.Salt;
            var hash = rfc2898.GetBytes(20);
            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string username, string password, string storedHash)
        {
            var hashBytes = Convert.FromBase64String(storedHash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using var rfc2898 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = rfc2898.GetBytes(20);

            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;


            var encryptedData = File.ReadAllBytes($"{username}.enc");
            var decryptedData = new AESEncryptor().DecryptBytes(encryptedData, $"7777");
            File.WriteAllBytes($"{username}.csv", decryptedData);

            return true;
        }
    }
}
