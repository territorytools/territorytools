using System;
using System.Collections.Generic;
using System.Linq;
using TerritoryTools.Web.Data;

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
        MainDbContext database;

        public TerritoryAuthorizationService(
            IEnumerable<string> userNames, IEnumerable<string> adminUserNames,
            MainDbContext database)
        {
            this.userNames = userNames;
            this.adminUserNames = adminUserNames;
            this.database = database;
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


            GetRoleForAlbaAccountIdFor(User.Identity.Name);


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
        public string GetRoleForAlbaAccountIdFor(string userName)
        {
            var accountLink = AccoundLinkFrom(userName);

            return accountLink.Role;
        }

        private Entities.TerritoryUserAlbaAccountLink AccoundLinkFrom(string userName)
        {
            var identityUser = database
               .Users
               .SingleOrDefault(u => u.NormalizedEmail == userName);

            if (identityUser == null)
            {
                throw new Exception(
                    $"An identity with the user name '{userName}' does not exist!");
            }

            var territoryUser = database
                .TerritoryUser
                .SingleOrDefault(u => u.AspNetUserId == identityUser.Id);

            if (territoryUser == null)
            {
                throw new Exception(
                    $"A territory user with identity user ID '{identityUser.Id}' does not exist! Account you lotted in with: {identityUser.Email}");
            }

            var accountLink = database
                .TerritoryUserAlbaAccountLink
                .FirstOrDefault(l => l.TerritoryUserId == territoryUser.Id);

            if (accountLink == null)
            {
                throw new Exception(
                    $"An Alba account link for territory user id '{territoryUser.Id}' does not exist!");
            }

            return accountLink;
        }
    }
}