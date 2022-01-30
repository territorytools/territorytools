using System;
using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAuthorizationService
    {
        bool IsAdmin(string userName);
        bool IsUser(string userName);
        IEnumerable<string> GetAdminUsers();
        IEnumerable<string> GetUsers();
    }

    public class TerritoryAuthorizationService : IAuthorizationService
    {
        IEnumerable<string> userNames;
        IEnumerable<string> adminUserNames;
        public TerritoryAuthorizationService(
            IEnumerable<string> userNames, 
            IEnumerable<string> adminUserNames)
        {
            this.userNames = userNames;
            this.adminUserNames = adminUserNames;
        }

        public bool IsAdmin(string userName)
        {
            foreach (string adminUserName in adminUserNames)
            {
                if (string.Equals(adminUserName, userName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // TODO: Check the database for Admin users too

            return false;
        }

        public bool IsUser(string userName)
        {
            foreach (string name in userNames)
            {
                if (string.Equals(name, userName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> GetAdminUsers()
        {
            return adminUserNames;
        }

        public IEnumerable<string> GetUsers()
        {
            return userNames;
        }
    }
}