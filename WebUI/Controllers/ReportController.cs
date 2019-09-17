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

                var allAssignments = GetAllAssignments(account, user, password)
                    .ToList();

                var now = DateTime.Now;
                int thisYear = now.Month >= 9 ? now.Year : (now.Year - 1);

                var report = new SummarizeCompletedReport()
                {
                    CompletedThisYear = allAssignments
                        .Where(a => a.LastCompleted >= new DateTime(thisYear, 9, 1))
                        .ToList()
                        .Count,
                    CompletedLastYear = allAssignments
                        .Where(a => a.LastCompleted >= new DateTime(thisYear-1, 9, 1)
                            && a.LastCompleted < new DateTime(thisYear, 9, 1))
                        .ToList()
                        .Count,
                    CompletedTwoYearsAgo = allAssignments
                        .Where(a => a.LastCompleted >= new DateTime(thisYear-2, 9, 1)
                            && a.LastCompleted < new DateTime(thisYear-1, 9, 1))
                        .ToList()
                        .Count,
                    CompletedThreeYearsAgo = allAssignments
                        .Where(a => a.LastCompleted >= new DateTime(thisYear-3, 9, 1)
                            && a.LastCompleted < new DateTime(thisYear-2, 9, 1))
                        .ToList()
                        .Count,
                    CompletedFourYearsAgo = allAssignments
                        .Where(a => a.LastCompleted >= new DateTime(thisYear-4, 9, 1)
                            && a.LastCompleted < new DateTime(thisYear-3, 9, 1))
                        .ToList()
                        .Count

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
