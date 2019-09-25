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

                var report = new SummarizeCompletedReport();

                for (int i = 0; i <= 10; i++)
                {
                    int year = thisYear - i;
                    report.SummaryItems.Add(
                        new SummaryItem
                        {
                            Name = $"{year}-{year + 1}",
                            Value = allAssignments
                                .Where(a => a.LastCompleted >= new DateTime(year, 9, 1)
                                && a.LastCompleted < new DateTime(year + 1, 9, 1))
                                .ToList()
                                .Count
                                .ToString()
                        });
                }

                return View(report);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
