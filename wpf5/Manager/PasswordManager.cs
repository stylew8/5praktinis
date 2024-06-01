using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using wpf5.Encryption;

namespace wpf5.Manager
{
    public class PasswordManager
    {

        public List<string[]> GetAllPasswords(string username)
        {
            if (!File.Exists($"{username}.csv")) return new List<string[]>();

            var lines = File.ReadAllLines($"{username}.csv");
            var results = new List<string[]>();

            foreach (var line in lines)
            {
                var columns = line.Split(',');
                results.Add(columns);
            }

            return results;
        }

        public void SavePassword(string title, string password, string url, string comment, string masterPassword, string username)
        {
            var encryptedPassword = new AESEncryptor().EncryptString(password, masterPassword);
            var line = $"{title},{encryptedPassword},{url},{comment}";
            File.AppendAllLines($"{username}.csv", new[] { line });
        }

        public List<string[]> SearchPassword(string title, string masterPassword , string username)
        {
            if (!File.Exists($"{username}.csv")) return new List<string[]>();

            var lines = File.ReadAllLines($"{username}.csv");
            var results = new List<string[]>();

            foreach (var line in lines)
            {
                var columns = line.Split(',');
                if (columns[0].Equals(title, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(columns);
                }
            }

            return results;
        }

        public void UpdatePassword(string title, string newPassword, string masterPassword, string username)
        {
            if (!File.Exists($"{username}.csv")) return;

            var lines = File.ReadAllLines($"{username}.csv");
            for (int i = 0; i < lines.Length; i++)
            {
                var columns = lines[i].Split(',');
                if (columns[0].Equals(title, StringComparison.OrdinalIgnoreCase))
                {
                    columns[1] = new AESEncryptor().EncryptString(newPassword, masterPassword);
                    lines[i] = string.Join(",", columns);
                    break;
                }
            }

            File.WriteAllLines($"{username}.csv", lines);
        }

        public void DeletePassword(string title, string username)
        {
            if (!File.Exists($"{username}.csv")) return;

            var lines = File.ReadAllLines($"{username}.csv").ToList();
            lines.RemoveAll(line => line.Split(',')[0].Equals(title, StringComparison.OrdinalIgnoreCase));
            File.WriteAllLines($"{username}.csv", lines);
        }

        public string GenerateRandomPassword(int length = 12)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new char[length];
            using var rng = new RNGCryptoServiceProvider();
            var uintBuffer = new byte[sizeof(uint)];

            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(uintBuffer);
                var num = BitConverter.ToUInt32(uintBuffer, 0);
                res[i] = valid[(int)(num % (uint)valid.Length)];
            }

            return new string(res);
        }
    }
}
