using System;
using System.Collections.Generic;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IAuthorizationServiceDeprecated
    {
        bool IsAdminDeprecated(string userName);
        bool IsUserDeprecated(string userName);
        IEnumerable<string> GetAdminUsersDeprecated();
        IEnumerable<string> GetUsers();
    }

    public class TerritoryAuthorizationService : IAuthorizationServiceDeprecated
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

        public bool IsAdminDeprecated(string userName)
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

        public bool IsUserDeprecated(string userName)
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

        public IEnumerable<string> GetAdminUsersDeprecated()
        {
            return adminUserNames;
        }

        public IEnumerable<string> GetUsers()
        {
            return userNames;
        }
    }
}