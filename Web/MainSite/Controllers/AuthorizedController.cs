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
        readonly IUserService _userService;
        readonly IAuthorizationService _authorizationService;

        public AuthorizedController(
            IUserService userService,
            IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor)
        {
            _userService = userService;
            _authorizationService = authorizationService;
            options = optionsAccessor.Value;
        }

        protected readonly WebUIOptions options;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        protected bool IsAdmin()
        {
            if (User.Identity.IsAuthenticated)
            {
                return _authorizationService.IsAdmin(User.Identity.Name);
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
