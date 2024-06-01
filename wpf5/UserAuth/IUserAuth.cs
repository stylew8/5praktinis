using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace wpf5.UserAuth
{
    internal interface IUserAuth
    {
        public bool RegisterUser(string username, string password);

        public bool LoginUser(string username, string password);
    }
}
