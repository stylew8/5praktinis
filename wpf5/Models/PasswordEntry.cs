using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf5.Models
{
    public class PasswordEntry
    {
        public string? Title { get; set; }
        public string? EncryptedPassword { get; set; }
        public string? Url { get; set; }
        public string? Comment { get; set; }
        public string DecryptedPassword { get; set; } = string.Empty;
    }
}
