using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebUI.Services
{
    public interface IAuthorizationService
    {
        bool IsAdmin(string userName);
        IEnumerable<string> GetAdminUsers();
    }

    public class TerritoryAuthorizationService : IAuthorizationService
    {
        IEnumerable<string> authorizedUserNames;
        public TerritoryAuthorizationService(IEnumerable<string> authorizedUserNames)
        {
            this.authorizedUserNames = authorizedUserNames;
        }

        public bool IsAdmin(string userName)
        {
            foreach (string authorizedName in authorizedUserNames)
            {
                if (string.Equals(authorizedName, userName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> GetAdminUsers()
        {
            return authorizedUserNames;
        }
    }
}