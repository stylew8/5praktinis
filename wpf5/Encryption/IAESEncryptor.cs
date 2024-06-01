using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf5.Encryption
{
    public interface IAESEncryptor
    {
        string EncryptString(string plainText, string password);
        string DecryptString(string cipherText, string password);
        byte[] EncryptBytes(byte[] plainBytes, string password);
        byte[] DecryptBytes(byte[] cipherBytes, string password);
    }
}
