using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using TerritoryTools.Web.MainSite.Models;
using TerritoryTools.Web.MainSite.Services;
using static TerritoryTools.Web.MainSite.BasicStrings;

namespace TerritoryTools.Web.MainSite.Controllers
{
    public class AuthorizedController : Controller
    {
        private readonly IUserFromApiService _userFromApiService;
        readonly IUserService _userService;
        readonly IAuthorizationService _authorizationService;

        public AuthorizedController(
            IUserFromApiService userFromApiService,
            IUserService userService,
            IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _userFromApiService = userFromApiService;
            _userService = userService;
            _authorizationService = authorizationService;
            _options = optionsAccessor.Value;
        }

        protected readonly WebUIOptions _options;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        protected bool IsAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _userFromApiService.ByEmail(User.Identity.Name);
                if (user != null && (user.IsActive ?? false)) 
                {
                    return true;
                }
            }

            return false;
        }

        protected bool IsUser()
        {
            var users = _userService.GetUsers(User.Identity.Name);

            foreach (var user in users)
            {
                if (StringsEqual(user.Email, User.Identity.Name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
