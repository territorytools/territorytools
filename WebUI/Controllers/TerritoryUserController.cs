using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using WebUI.Areas.Identity.Data;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Controllers
{
    [Authorize]
    public class TerritoryUserController : AuthorizedController
    {
        public TerritoryUserController(
            MainDbContext database,
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            Services.IAuthorizationService authorizationService,
            IAlbaCredentialService albaCredentialService,
            IOptions<WebUIOptions> optionsAccessor) 
            : base(
                database,
                localizer,
                credentials,
                authorizationService,
                albaCredentialService,
                optionsAccessor)
        {
        }

        public IActionResult Index()
        {
            if (!IsUser())
            {
                return Forbid();
            }

            return View();
        }

        public IActionResult Invitation()
        {
            try
            {
                if (!IsUser())
                {
                    return Forbid();
                }

                var invitation = new TerritoryUserInvitation
                {
                };

                return View(null);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Invitation(TerritoryUserInvitation invitation)
        {
            if (!IsUser())
            {
                return Forbid();
            }

            if (database.TerritoryUser.Count(u => BasicStrings.StringsEqual(u.Email, invitation.Email)) > 0)
            {
                return RedirectToAction(nameof(AlreadyInvited), invitation);
            }

            var now = DateTime.Now;

            var user = new TerritoryUser
            {
                Id = Guid.NewGuid(),
                Email = invitation.Email,
                Surname = invitation.Surname,
                GivenName = invitation.GivenName,
                Created = now,
                Updated = now,
                Role = "Invited"
            };

            database.TerritoryUser.Add(user);
            database.SaveChanges();

            return Ok();
        }

        public IActionResult AlreadyInvited(TerritoryUserInvitation invitation)
        {
            try
            {
                if (!IsUser())
                {
                    return Forbid();
                }

                return View(invitation);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
