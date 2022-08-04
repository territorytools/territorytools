﻿using System;

namespace TerritoryTools.Alba.Controllers.Models
{
    public class Credentials
    {
        public Credentials(string account, string user, string password)
        {
            Account = account;
            User = user;
            Password = password;
        }

        public string Host { get; set; }
        public string Account;
        public string User;
        public string Password;
        public Guid AlbaAccountId;
        public string K1MagicString;
        public string SessionKeyValue;

        public string Combined { get { return Account + User + Password + K1MagicString; } } 
    }
}