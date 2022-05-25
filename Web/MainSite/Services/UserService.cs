using cuc = Controllers.UseCases;
using System.Collections.Generic;
using System;
using Controllers.UseCases;
using Microsoft.Extensions.Options;

namespace TerritoryTools.Web.MainSite.Services
{
    public interface IUserService
    {
        List<cuc.User> GetUsers(string authenticatedUserName);
    }

    public class UserService : IUserService
    {
        readonly WebUIOptions options;
        readonly IAlbaCredentialService _albaCredentialService;
        readonly IAuthorizationService _authorizationService;

        public UserService(
            IAlbaCredentialService albaCredentialService,
            IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            options = optionsAccessor.Value;
            _albaCredentialService = albaCredentialService;
            _authorizationService = authorizationService;
        }

        public  List<cuc.User> GetUsers(string authenticatedUserName)
        {
            var users = new List<cuc.User>();

            string currentUser = authenticatedUserName;
            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                Guid albaAccountId = _albaCredentialService.GetAlbaAccountIdFor(currentUser);
                string path = string.Format(
                   options.AlbaUsersHtmlPath,
                   albaAccountId);

                if (System.IO.File.Exists(path))
                {
                    string html = System.IO.File.ReadAllText(path);
                    users = DownloadUsers.GetUsers(html);
                }
            }

            var adminUserNames = _authorizationService.GetAdminUsers();
            foreach (var name in adminUserNames)
            {
                users.Add(
                    new cuc.User
                    {
                        Email = name,
                        Name = name,
                    });
            }

            var userNames = _authorizationService.GetUsers();
            foreach (var name in userNames)
            {
                users.Add(
                    new cuc.User
                    {
                        Email = name,
                        Name = name,
                    });
            }

            return users;
        }

    }
}
