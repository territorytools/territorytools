using System;

namespace AlbaClient.Models
{
    public class Credentials
    {
        public Credentials(string account, string user, string password, string k1MagicString)
        {
            Account = account;
            User = user;
            Password = password;
            K1MagicString = k1MagicString;
        }

        public string Account;
        public string User;
        public string Password;
        public Guid AlbaAccountId;
        public string K1MagicString;

        public string Combined { get { return Account + User + Password + K1MagicString; } } 
    }
}