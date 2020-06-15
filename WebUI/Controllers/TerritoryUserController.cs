using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using WebUI.Areas.Identity.Data;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Controllers
{
    public class TerritoryUserController : AuthorizedController
    {
        public TerritoryUserController(
            MainDbContext database,
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            IAuthorizationService authorizationService,
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
            return View();
        }

        public IActionResult Invitation()
        {
            try
            {
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
                return View(invitation);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
