using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WebUI.Controllers
{
    [Authorize]
    public class ReportController : AuthorizedController
    {
        public ReportController(
            IStringLocalizer<AuthorizedController> localizer,
            IAlbaCredentials credentials,
            Services.IAuthorizationService authorizationService,
            IOptions<WebUIOptions> optionsAccessor) : base(
                localizer,
                credentials,
                authorizationService,
                optionsAccessor)
        {
        }

        [Authorize]
        public IActionResult Index()
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            return View();
        }

        [Authorize]
        public IActionResult SummarizeCompleted()
        {
            try
            {
                if (!IsAdmin())
                {
                    return Forbid();
                }

                var assignments = GetAllAssignments(account, user, password)
                    .Where(a => a.LastCompleted >= new DateTime(2019,9,1) )
                    .ToList();

                var report = new SummarizeCompletedReport()
                {
                    CompletedCount = assignments.Count
                };

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
