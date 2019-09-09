using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI
{
    public interface IAlbaCredentials
    {
        string Account { get; set; }
        string User { get; set; }
        string Password { get; set; }
    }

    public class AlbaCredentials : IAlbaCredentials
    {
        public AlbaCredentials(string account, string user, string password)
        {
            Account = account;
            User = user;
            Password = password;
        }

        public string Account { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
