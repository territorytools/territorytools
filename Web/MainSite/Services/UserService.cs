using Microsoft.Extensions.Options;
using System.Collections.Generic;
using cuc = Controllers.UseCases;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IUserService
    {
        List<cuc.User> GetUsers(string authenticatedUserName);
        void LoadUsers(string userName);
    }

    public class UserService : IUserService
    {
        readonly WebUIOptions options;
        readonly IAlbaUserGateway _albaUserGateway;
        readonly IAuthorizationService _authorizationService;

        public UserService(
            IAlbaUserGateway albaUserGateway,
            IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            options = optionsAccessor.Value;
            _albaUserGateway = albaUserGateway;
            _authorizationService = authorizationService;
        }

        public  List<cuc.User> GetUsers(string authenticatedUserName)
        {
            var users = new List<cuc.User>();

            string currentUser = authenticatedUserName;
            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                users = _albaUserGateway.GetAlbaUsers(currentUser);
            }

            var adminUserNames = _authorizationService.GetAdminUsers();
            foreach (string name in adminUserNames)
            {
                if (users.Exists(u => string.Equals(u.Email, name)))
                {
                    continue;
                }

                users.Add(
                    new cuc.User
                    {
                        Email = name,
                        Name = name,
                    });
            }

            var userNames = _authorizationService.GetUsers();
            foreach (string name in userNames)
            {
                if (users.Exists(u => string.Equals(u.Email, name)))
                {
                    continue;
                }

                users.Add(
                    new cuc.User
                    {
                        Email = name,
                        Name = name,
                    });
            }

            return users;
        }

        public void LoadUsers(string userName)
        {
            _albaUserGateway.LoadAlbaUsers(userName);
        }
    }
}
